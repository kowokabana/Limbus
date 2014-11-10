using System;
using NetMQ;
using System.Threading;
using System.IO;
using NetMQ.Sockets;
using Limbus.Log;

namespace Limbus.Swarm
{
	public class FileClient
	{
		private const int PartSize = 1024;

		private void SendFile(RequestSocket client, string path)
		{
			var filename = Path.GetFileName(path);
			client.Send(filename);
			var answer = client.ReceiveString();

			using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{					
				int rest = (int)fileStream.Length;
				while (rest >= PartSize)
				{
					rest -= PartSize;
					var part = new byte[PartSize];
					fileStream.Read(part, 0, PartSize);
					client.Send(part, PartSize, false, true);
				}

				var lastPart = new byte[rest];
				fileStream.Read(lastPart, 0, rest);
				client.Send(lastPart, rest, false, false);
			}
		}

		public FileClient(string address, string path)
		{
			using (NetMQContext ctx = NetMQContext.Create())
			{
				using (var client = ctx.CreateRequestSocket())
				{
					client.Connect(address);
					Logger.Log("Connected to ..");
					SendFile(client, path);
				}
			}
		}
	}
}

