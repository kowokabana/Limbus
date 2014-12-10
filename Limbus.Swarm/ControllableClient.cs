using System;
using Limbus.Control;
using NetMQ;
using System.Threading;
using Limbus.API;

namespace Limbus.Swarm
{
	public class ControllableClient<T>
	{
		public ControllableClient(IControllable<T> controllable, string address, string name)
		{
			using (NetMQContext ctx = NetMQContext.Create())
			{
				using (var client = ctx.CreateRequestSocket())
				{
					client.Connect(address);

					while (true)
					{
						client.Send(string.Format("Hello from client {0}", name));
						string fromServerMessage = client.ReceiveString();
						Console.WriteLine("From Server: {0} running on ThreadId : {1}", 
							fromServerMessage, Thread.CurrentThread.ManagedThreadId);
						Thread.Sleep(5000);
					}
				}
			}
		}
	}
}

