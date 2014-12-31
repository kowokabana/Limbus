using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Limbus.API;
using MoreLinq;
using System.Threading.Tasks;

namespace Limbus.Arduino
{
	public class Driver : IDisposable
	{
		public int SamplingRate { get; set; }

		private Dictionary<string, PinPair> PinPairs;
		private SerialPort port = null;
		private Task reader;
		private object mutex = new object();

		public Driver()
		{
			PinPairs = new Dictionary<string, PinPair>();
			SamplingRate = 200;
		}

		public void Dispose()
		{
			if (port != null) {
				reader.Dispose();
				port.Close();
				port.Dispose();
			}
		}

		public IControllable<double> AddPinPair(string inPin, string outPin)
		{
			var pinPair = new PinPair(inPin, outPin);
			lock (mutex)
			{
				this.PinPairs.Add(inPin + outPin, pinPair);
			}
			return pinPair;
		}

		public IControllable<double> Pin(string inPin, string outPin)
		{
			lock (mutex)
			{
				return PinPairs[inPin + outPin];
			}
		}

		public bool Connect(string portName, int baudRate)
		{
			if (!SerialPort.GetPortNames().Contains(portName)) return false;
			this.port = new SerialPort(portName, baudRate);
			//port.ReadTimeout = 500;
			//port.WriteTimeout = 500;
		  port.Open();
			PinPairs.Values.ForEach(p => p.Port = port);
			return true;
		}

		private void Read()
		{
			while (true)
			{
				try
				{
					lock(mutex)
					{
						string message = port.ReadLine();
						PinPairs.Values.ForEach(p => p.Read(message));
					}
					port.DiscardInBuffer();
					Thread.Sleep(SamplingRate);
				}
				catch (TimeoutException ex)
				{
				}
			}
		}

		public void StartReading()
		{
			reader = Task.Run(() => Read());
		}
	}
}

