using System;
using System.Threading.Tasks;
using NetMQ;
using System.Threading;
using System.IO;
using NetMQ.Sockets;

namespace Limbus.Swarm
{
	public class FileServer
	{
		private void ReceiveFile(ResponseSocket server, string path)
		{
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

		public FileServer(string address, string path)
		{
			Task.Run(() => {
				using (NetMQContext ctx = NetMQContext.Create())
				{
					using (var server = ctx.CreateResponseSocket())
					{
						server.Bind(address);
						ReceiveFile(server, path);
					}
				}
			});
		}
	}
}

