using System;
using System.Collections.Generic;
using Limbus.Clockwork;
using Limbus.Allocation;

namespace Limbus.Control
{
	public interface IController<T1, T2>
	{
		void Update(IEnumerable<IControllable<T1>> swarm, IAllocator<T1, T2> allocator);
	}
}

