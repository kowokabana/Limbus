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
	}
}

