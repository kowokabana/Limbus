using System;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Threading;
using NetMQ;
using Limbus.Clockwork;
using Limbus.Mosquito;

namespace Limbus.Swarm.Test
{
	[TestFixture]
	public class SwarmTests
	{
		[Test]
		[Ignore]
		public void Copy_file()
		{
			var address = "tcp://127.0.0.1:5556";

			Task.Run(() => new FileServer(address,"/Users/kowo/Downloads/snarf_copied.zip"));
			Task.Run(() => new FileClient(address, "/Users/kowo/Downloads/snarf.zip"));

			Thread.Sleep(3.s());
		}

		[Test]
		[Ignore]
		public void Remote_mosquito()
		{
			var address = "tcp://127.0.0.1:5556";
			Task.Run(() => {
				using (NetMQContext ctx = NetMQContext.Create())
				{
					using (var server = ctx.CreateResponseSocket())
					{
						server.Bind(address);

						while (true)
						{
							string fromClientMessage = server.ReceiveString();
							Console.WriteLine("From Client: {0} running on ThreadId : {1}", 
								fromClientMessage, Thread.CurrentThread.ManagedThreadId);
							server.Send("Hi Back");
						}
					}
				}
			});

			Task.Run(() =>
				new ControllableClient<double>(new LinearMosquito(
					2.0.In(1.min()), 
					DateTimeOffset.UtcNow),
					address,
					"ClientA"));

			Task.Run(() => 
				new ControllableClient<double>(new LinearMosquito(
					2.0.In(1.min()), 
					DateTimeOffset.UtcNow), 
					address,
					"ClientB"));

			Thread.Sleep(20.s());
		}
	}
}

