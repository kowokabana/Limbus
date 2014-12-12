using System;

namespace Limbus.API
{
	public class SpecifiedAllocatable<T, T2> : IAllocatable<T>, ISpecified<T2>, ISpecifiedAllocatable<T, T2>
	{
		public void Allocate(T quantity)
		{
			this.Quantity = quantity;
		}
		public T Quantity { get; private set; }
		public T2 Spec { get; private set; }

		public SpecifiedAllocatable(T2 spec)
		{
			Spec = spec;
		}
	}
}

