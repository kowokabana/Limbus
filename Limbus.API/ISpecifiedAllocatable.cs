using System;

namespace Limbus.API
{
	public interface ISpecifiedAllocatable<T, T2> : IAllocatable<T>, ISpecified<T2>
	{
	}
}

