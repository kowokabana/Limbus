using System;

namespace Limbus.Clockwork
{
	public interface ITickable
	{
		DateTimeOffset Tick (TimeSpan step);
	}
}

