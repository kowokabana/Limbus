using System;

namespace Limbus.API
{
	public interface ISpecified<T>
	{
		T Spec { get; }
	}
}

