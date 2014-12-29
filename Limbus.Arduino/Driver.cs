using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Limbus.API;
using MoreLinq;

namespace Limbus.Arduino
{
	public class Driver : IDisposable
	{
		private Dictionary<string, Pin> Pins;
		private SerialPort port = null;

		public Driver()
		{
			Pins = new Dictionary<string, Pin>();
		}

		public void Dispose()
		{
			if (port != null) {
				this.Pins.ForEach(p => p.Value.Dispose());
				port.Close();
				port.Dispose();
			}
		}

		public IControllable<double> AddPin(string name)
		{
			var pin = new Pin(this.port, name);
			this.Pins.Add(name, pin);
			return pin;
		}

		public IControllable<double> Pin(string pin)
		{
			return Pins[pin];
		}

		public bool Connect(string portName, int baudRate)
		{
			if (!SerialPort.GetPortNames().Contains(portName)) return false;
			this.port = new SerialPort(portName, baudRate);
			//port.ReadTimeout = 500;
			//port.WriteTimeout = 500;
		  port.Open();
			Pins.ForEach(p => p.Value.StartReading(this.port));
			return true;
		}

		//string potVal = "90\r\nal: 229\r\npotVal: 236\r\npotVal: 239\r\npotVal: 229\r\npotVal: 232\r\npotVal: 0\r\npotVal: 241\r\npotVal: 230\r\npotVal: 238\r\npotVal: 240\r\npotVal: 229\r\npotVal: 236\r\npotVal: 239\r\npotVal: 229\r\npotVal: 232\r\npotVal: 0\r\npotVal: 241\r\npotVal: 230\r\npotVal: 238\r\npotVal: 240\r\npotVal: 229\r\npotVal: 236\r\npotVal: 239\r\npotVal: 229\r\npotVal: 232\r\npotVal: potVal: 251\r\npotVal: 250\r\npotVal: 250\r\npotVal: 250\r\npotVal: 250\r\npotVal: 249\r\npotVal: 246\r\npotVal: 246\r\npotVal: 247\r\npotVal: 241\r\npotVal: 242\r\npotVal: 241\r\npotVal: 237\r\npotVal: 238\r\npotVal: 237\r\npotVal: 234";
//		public bool Connect2(string portName, int baudRate)
//		{
//			if (!SerialPort.GetPortNames().Contains(portName)) return false;
//			using (var port = new SerialPort(portName, baudRate)) {
//				port.Open();
//
//				/*byte[] buffer = new byte[3];
//				buffer[0] = Convert.ToByte(4);
//				buffer[1] = Convert.ToByte(8);
//				buffer[2] = Convert.ToByte(16);
//
//				port.Write(buffer, 0, 3);
//				Thread.Sleep(200);*/
//
//				string answer = String.Empty;
//				var answerSize = port.BytesToRead;
//
//				while (answerSize > 0) {
//					var readByte = port.ReadByte();
//					answer += Convert.ToChar(readByte);
//					answerSize--;
//				}
//				port.Close();
//				//if (!answer.Equals("Hello Arduino")) return false;
//			}
//			return true;
//		}
	}
}

