using System;
using System.Collections.Generic;
using Limbus.Clockwork;

namespace Limbus.API
{
	public interface IController<T, T2>
	{
		void Update(IEnumerable<ControlEntity<T, T2>> swarm, IAllocator<T, T2> allocator);
	}
}

