using System;

namespace Limbus.Clockwork
{
	public class Timestamped<T> : IEquatable<Timestamped<T>>
	{
		public DateTimeOffset Timestamp { get; private set; }
		public T Value { get; private set; }

		public Timestamped(T v, DateTimeOffset time)
		{
			this.Timestamp = time;
			this.Value = v;
		}

		public bool Equals(Timestamped<T> other)
		{
			return Value.Equals(other.Value) && Timestamp.Equals(other.Timestamp);
		}
	}

	public static class Timestamped
	{
		public static Timestamped<T> Create<T>(T v, DateTimeOffset timestamp)
		{
			return new Timestamped<T>(v, timestamp);
		}

		public static Timestamped<T> At<T>(this T v, DateTimeOffset timestamp)
		{
			return Timestamped.Create<T>(v, timestamp);
		}
	}
}

