using System;

namespace Limbus.Clockwork
{
	public class TimeSpaned<T>
	{
		public TimeSpan Duration { get; private set; }
		public T Value { get; private set; }

		public TimeSpaned (T val, TimeSpan duration)
		{
			this.Duration = duration;
			this.Value = val;
		}
	}
}

