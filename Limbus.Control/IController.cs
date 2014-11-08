using System;
using System.Collections.Generic;

namespace Limbus.Control
{
	public interface IController<T> : IControllable<T>
	{
		void Update(IEnumerable<IControllable<T>> controllables);
	}
}

