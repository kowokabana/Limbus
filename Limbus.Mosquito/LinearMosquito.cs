using System;
using Limbus.Clockwork;
using Limbus.Control;
using System.Collections.Generic;

namespace Limbus.Mosquito
{
	/// <summary>
	/// A thread-safe mosquito with a linear-behaving engine
	/// </summary>
	public class LinearMosquito : IControllable<double>, ITimed, IEngine<double>
	{
		public event Action<Timestamped<double>> Receive;

		public Timestamped<double> Setpoint { get; private set; }
		public TimeSpaned<double> Gradient { get; private set; }

		private object mutex = new object();
		private DateTimeOffset time;
		private double actual = 0;

		public LinearMosquito(TimeSpaned<double> gradient, DateTimeOffset now)
		{
			if (gradient.Value <= 0) throw new ArgumentOutOfRangeException ("gradient");

			this.time = now;
			this.Setpoint = 0.0.At(this.time);
			this.Gradient = gradient;
		}

		public void Send(Timestamped<double> setpoint)
		{
			lock (mutex)
			{
				this.actual = GetActual();
				var earliestFinishTime = this.time.Add(actual.TimeTo(setpoint.Value, Gradient));

				this.Setpoint = setpoint.Timestamp > earliestFinishTime ?
					setpoint : 
					setpoint.Value.At(earliestFinishTime);
			}
		}

		private double GetActual()
		{
			if (time >= Setpoint.Timestamp) return Setpoint.Value;
			var engineStartTime = Setpoint.Timestamp - this.actual.TimeTo(Setpoint.Value, Gradient);
			if (time < engineStartTime) return this.actual;

			var timeSinceStart = time - engineStartTime;
			var delta = (timeSinceStart.TotalSeconds / Gradient.Duration.TotalSeconds) * Gradient.Value;
			return Setpoint.Value > this.actual ? 
				this.actual + delta : 
				this.actual - delta;
		}

		public void Set(DateTimeOffset time)
		{
			lock (mutex)
			{
				this.time = time;
				if (Receive != null) Receive(GetActual().At(time));
			}
		}
	}
}

