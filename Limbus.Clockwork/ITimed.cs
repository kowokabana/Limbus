using System;

namespace Limbus.Clockwork
{
	public interface ITimed
	{
		void Set(DateTimeOffset time);
	}
}

