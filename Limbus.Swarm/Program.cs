using System;
using NetMQ;
using Limbus.Mosquito;
using Limbus.Clockwork;
using System.Threading.Tasks;
using System.Threading;

namespace Limbus.Swarm
{
	class Program
	{
		private static void Main(string[] args)
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
				TimeSpan.Zero,
				DateTimeOffset.UtcNow),
				address,
				"ClientA"));

			Task.Run(() => 
				new ControllableClient<double>(new LinearMosquito(
					2.0.In(1.min()), 
					TimeSpan.Zero,
					DateTimeOffset.UtcNow), 
					address,
					"ClientB"));

			Thread.Sleep(20.min());
		}
	}
}

