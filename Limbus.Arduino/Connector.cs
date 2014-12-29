using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace Limbus.Arduino
{
	public class Connector
	{
		public bool Connect(string portName, int baudRate)
		{
			if (!SerialPort.GetPortNames().Contains(portName)) return false;
			using (var port = new SerialPort(portName, baudRate)) {
				port.Open();

				byte[] buffer = new byte[3];
				buffer[0] = Convert.ToByte(4);
				buffer[1] = Convert.ToByte(8);
				buffer[2] = Convert.ToByte(16);

				port.Write(buffer, 0, 3);
				Thread.Sleep(200);

				string answer = String.Empty;
				var answerSize = port.BytesToRead;
				while (answerSize > 0) {
					var readByte = port.ReadByte();
					answer += Convert.ToChar(readByte);
					answerSize--;
				}
				port.Close();
				if (!answer.Equals("Hello Arduino")) return false;
			}
			return true;
		}
	}
}

