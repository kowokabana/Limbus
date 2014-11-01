using System;

namespace Limbus.Clockwork
{
	public class Clock : ITimed
	{
		public DateTimeOffset Time { get; private set; }
		private event Action<DateTimeOffset> Reset;

		public Clock (DateTimeOffset time)
		{
			Time = time;
		}

		public DateTimeOffset Tick (TimeSpan timeSpan)
		{
			Time = Time.Add (timeSpan);
			if (Reset != null) Reset (Time);
			return Time;
		}

		public void Set(DateTimeOffset time)
		{
			Time = time;
			if (Reset != null) Reset (Time);
		}

		public IDisposable Subscribe(ITimed timed)
		{
			Reset += timed.Set;
			return new Action(() => Reset -= timed.Set).AsDisposable();
		}

		public void Unsubscribe(ITimed timed)
		{
			Reset -= timed.Set;
		}
	}
}

