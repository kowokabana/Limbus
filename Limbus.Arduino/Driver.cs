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
	}
}

