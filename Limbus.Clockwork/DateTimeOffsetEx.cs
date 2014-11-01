using System;

namespace Limbus.Clockwork
{
	public static class DateTimeOffsetEx
	{
		public static DateTimeOffset Create()
		{
			return new DateTimeOffset (0, 0, 0, 0, 0, 0, 0, TimeSpan.Zero);
		}

		public static DateTimeOffset min(this DateTimeOffset src, int min)
		{
			return src.Add (min.min ());
		}

		public static DateTimeOffset s(this DateTimeOffset src, int s)
		{
			return src.Add (s.s ());
		}

		public static DateTimeOffset ms(this DateTimeOffset src, int ms)
		{
			return src.Add (ms.ms ());
		}
	}
}

