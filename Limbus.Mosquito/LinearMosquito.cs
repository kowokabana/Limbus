using System;
using Limbus.Clockwork;
using System.Reactive;
using Limbus.Control;

namespace Limbus.Mosquito
{
	public class LinearMosquito : IControllable<double>, ITimed
	{
		public event Action<Timestamped<double>> Receive;
		private Timestamped<double> Setpoint;
		private TimeSpaned<double> Gradient;
		private TimeSpan duration;

		public LinearMosquito (TimeSpaned<double> gradient)
		{
			this.Gradient = gradient;
		}

		public void Send (Timestamped<double> setpoint)
		{
			this.Setpoint = setpoint;
			this.duration = TimeSpan.FromTicks((long)((Setpoint.Value / Gradient.Value) * Gradient.Duration.Ticks));
		}

		public double GetSetpoint(DateTimeOffset now)
		{
			if (now >= Setpoint.Timestamp) return Setpoint.Value;
			var setpointStart = Setpoint.Timestamp - duration;
			if (now < setpointStart) return 0.0;

			var timeSinceStart = now - setpointStart;
			double setpointNow = (timeSinceStart.TotalSeconds / Gradient.Duration.TotalSeconds) * Gradient.Value;
			return setpointNow;
		}

		public void Set(DateTimeOffset now)
		{
			if (Receive != null) Receive(Timestamped.Create<double>(GetSetpoint(now), now));
		}
	}
}

