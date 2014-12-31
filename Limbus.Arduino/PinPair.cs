using System;
using Limbus.API;
using Limbus.Clockwork;
using System.IO.Ports;

namespace Limbus.Arduino
{
	public class PinPair : IControllable<double>
	{
		public SerialPort Port { get; set; }
		private string inPin;
		private string outPin;

		public PinPair(string inPin, string outPin)
		{
			this.inPin = inPin;
			this.outPin = outPin;
		}

		public event Action<Timestamped<double>> Receive;

		public void Send(Timestamped<double> setpoint)
		{
			var val = outPin + ":" + setpoint.Value.ToString() + ";";
			if(Port != null) Port.Write(val);
		}

		public void Read(string message)
		{
			if (!message.StartsWith(inPin)) return;
			var valStr = message.Substring(inPin.Length);
			double val = 0.0;
			if (!double.TryParse(valStr, out val)) return;
			if (Receive != null) Receive((val).At(DateTimeOffset.UtcNow));
		}
	}
}

