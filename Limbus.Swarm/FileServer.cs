using System;
using System.Threading.Tasks;
using NetMQ;
using System.Threading;
using System.IO;

namespace Limbus.Swarm
{
	public class FileServer
	{
		public FileServer(string address, string path)
		{
			Task.Run(() => {
				using (NetMQContext ctx = NetMQContext.Create())
				{
					using (var server = ctx.CreateResponseSocket())
					{
						server.Bind(address);
						using(var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
						{
							bool more = false;
							do
							{
								var part = server.Receive(out more);
								fileStream.Write(part, 0, part.Length);
							} 
							while(more);
						}
					}
				}
			});
		}
	}
}

