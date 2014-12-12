using System;
using System.Collections.Generic;
using Limbus.API;

namespace Limbus.Allocation
{
	public interface IAllocator<T, T2>
	{
		bool TryAllocate(IEnumerable<ISpecifiedAllocatable<T, T2>> allocatables, T setpoint);
	}
}

