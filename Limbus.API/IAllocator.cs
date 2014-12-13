using System;
using System.Collections.Generic;

namespace Limbus.API
{
	public interface IAllocator<T, T2>
	{
		bool TryAllocate(IEnumerable<ISpecifiedAllocatable<T, T2>> allocatables, T setpoint);
	}
}

