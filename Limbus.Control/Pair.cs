using System;
using Limbus.Clockwork;

namespace Limbus.Control
{
	public class Pair
	{
		public Timestamped<double> Setpoint { get; set; }
		public Timestamped<double> Actual { get; set; }

		public TimeSpan dt { get { return this.Setpoint.Timestamp - this.Actual.Timestamp; } }
		public double dv { get { return this.Setpoint.Value - this.Actual.Value; } }

		public Pair(Timestamped<double> setpoint, Timestamped<double> actual)
		{
			this.Setpoint = setpoint;
			this.Actual = actual;
		}
	}
}

