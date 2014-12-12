using System;

namespace Limbus.API
{
	public interface IAllocatable<T>
	{
		void Allocate(T quantity);
		T Quantity { get; }
	}
}

