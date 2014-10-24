using System;

namespace Limbus.Clockwork
{
	public class TimeSpaned<T>
	{
		public TimeSpan Duration { get; private set; }
		public T Value { get; private set; }

		public TimeSpaned (TimeSpan duration, T val)
		{
			this.Duration = duration;
			this.Value = val;
		}
	}
}

