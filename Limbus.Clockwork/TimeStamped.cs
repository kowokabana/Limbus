using System;

namespace Limbus.Clockwork
{
	public class Timestamped<T>
	{
		public DateTimeOffset Timestamp { get; private set; }
		public T Value { get; private set; }

		public Timestamped (T v, DateTimeOffset time)
		{
			this.Timestamp = time;
			this.Value = v;
		}
	}

	public static class Timestamped
	{
		public static Timestamped<T> Create<T>(T v, DateTimeOffset time)
		{
			return new Timestamped<T>(v, time);
		}
	}
}

