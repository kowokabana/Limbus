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
			var val = setpoint.Value.ToString() + ";";
			this.port.Write(val);
		}

		private void Read()
		{
			while (true)
			{
				try
				{
					string message = port.ReadLine();
					if(!message.StartsWith(name)) continue;
					var valStr = message.Substring(name.Length);
					double val = 0.0;
					if(!double.TryParse(valStr, out val)) continue;
					if (Receive != null) Receive((val-100).At(DateTimeOffset.UtcNow));
					port.DiscardInBuffer();
					Thread.Sleep(200);
				}
				catch (TimeoutException ex)
				{
				}
			}
		}
	}
}

