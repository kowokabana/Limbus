using System;

namespace Limbus.Clockwork
{
	public interface ITimed
	{
		void Set (DateTimeOffset time);
	}

	public static class TimedExtensions
	{
		public static ITimed TimedBy(this ITimed src, Clock clock)
		{
			clock.Subscribe (src);
			return src;
		}
	}
}

