using System;
using NetMQ;
using System.Threading;
using System.IO;

namespace Limbus.Swarm
{
	public class FileClient
	{
		private const int PartSize = 1024;

		public FileClient(string address, string path)
		{
			using (NetMQContext ctx = NetMQContext.Create())
			{
				using (var client = ctx.CreateRequestSocket())
				{
					client.Connect(address);

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
			}
		}
	}
}

