using System;
using System.Collections.Generic;

namespace Limbus.Allocation
{
	public interface IAllocator<T1, T2>
	{
		bool TryAllocate(IEnumerable<Allocatable<T1, T2>> allocatables, T1 setpoint);
	}
}

