using System;
using Limbus.API;
using Limbus.Clockwork;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;

namespace Limbus.Arduino
{
	public class Pin : IControllable<double>, IDisposable
	{
		private SerialPort port;
		private string name;
		private Task reader;

		public void Dispose()
		{
			StopReading();
		}

		public void StartReading(SerialPort port)
		{
			this.port = port;
			reader = Task.Run(() => Read());
		}

		public void StopReading()
		{
			reader.Dispose();
		}

		public Pin(SerialPort port, string name)
		{
			// Set the read/write timeouts
			//port.ReadTimeout = 500;
			//port.WriteTimeout = 500;

			this.port = port;
			this.name = name;
		}

		public event Action<Timestamped<double>> Receive;

		public void Send(Timestamped<double> setpoint)
		{
			this.port.Write(name += ": " + setpoint.Value.ToString());
		}

		private void Read()
		{
			while (true)
			{
				try
				{
					string message = port.ReadLine();
					if(!message.StartsWith(name)) continue;
					var valStr= message.Substring(name.Length + 1);
					double val = 0.0;
					if(!double.TryParse(valStr, out val)) continue;
					if (Receive != null) Receive(val.At(DateTimeOffset.UtcNow));
					port.DiscardInBuffer();
					Thread.Sleep(100);
				}
				catch (TimeoutException ex)
				{
				}
			}
		}
	}
}

