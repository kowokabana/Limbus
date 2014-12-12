using System;
using System.Collections.Generic;
using Limbus.Clockwork;
using Limbus.Allocation;
using Limbus.API;

namespace Limbus.Control
{
	public interface IController<T, T2>
	{
		void Update(IEnumerable<ControlEntity<T, T2>> swarm, IAllocator<T, T2> allocator);
	}
}

