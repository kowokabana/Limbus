using System;
using Limbus.Clockwork;
using System.Reactive;
using Limbus.Control;
using System.Collections.Generic;

namespace Limbus.Mosquito
{
	public class LinearMosquito : IControllable<double>, ITimed
	{
		public event Action<Timestamped<double>> Receive;

		public Timestamped<double> Setpoint { get; private set; }
		public TimeSpaned<double> Gradient { get; private set; }
		public TimeSpan Duration { get; private set; }
		public DateTimeOffset Time { get; private set; }

		private object mutex = new object();

		public LinearMosquito (TimeSpaned<double> gradient)
		{
			if (gradient.Value <= 0) throw new ArgumentOutOfRangeException ("gradient");

			this.Setpoint = Timestamped.Create<double> (0, DateTimeOffset.UtcNow);
			this.Gradient = gradient;
			this.Time = DateTimeOffset.UtcNow;
			this.Duration = TimeSpan.Zero;
		}

		public void Send (Timestamped<double> setpoint)	
		{
			lock (mutex)
			{
				this.Duration = GetDurationTo (setpoint.Value);
				this.Setpoint = setpoint; // changing this.Setpoint influences GetActual()
			}
		}

		private TimeSpan GetDurationTo(double v)
		{
			var dv = Math.Abs (v - GetActual ());
			return TimeSpan.FromTicks((long)((dv / Gradient.Value) * Gradient.Duration.Ticks));
		}

		private double GetActual()
		{
			if (Time >= Setpoint.Timestamp) return Setpoint.Value;
			var setpointStart = Setpoint.Timestamp - Duration;
			if (Time < setpointStart) return 0.0;

			var timeSinceStart = Time - setpointStart;
			double setpointNow = (timeSinceStart.TotalSeconds / Gradient.Duration.TotalSeconds) * Gradient.Value;
			return setpointNow;
		}

		public void Set(DateTimeOffset time)
		{
			lock (mutex)
			{
				Time = time;
				if (Receive != null) Receive(Timestamped.Create<double>(GetActual(), Time));
			}
		}
	}
}

