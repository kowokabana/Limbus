using System;

namespace Limbus.Clockwork
{
	public static class TimeSpanExtensions
	{ 
		public static TimeSpan min(this int src)
		{
			return TimeSpan.FromMinutes (src);
		}

		public static TimeSpan s(this int src)
		{
			return TimeSpan.FromSeconds (src);
		}

		public static TimeSpan TimeTo(this double from, double to, TimeSpaned<double> gradient)
		{
			var dv = Math.Abs (to - from);
			return TimeSpan.FromTicks((long)((dv / gradient.Value) * gradient.Duration.Ticks));
		}
	}
}

