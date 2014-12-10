using System;
using System.Collections.Generic;

namespace Limbus.Allocation
{
	public interface IAllocator<T, T2>
	{
		bool TryAllocate(IEnumerable<AllocatableThing<T, T2>> allocatables, T setpoint);
	}
}

