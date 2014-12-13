using System;

namespace Limbus.API
{
	public interface IControlAlgorithm<T>
	{
		bool TryControl(DateTimeOffset t, T y, out T r);
		void Reset(DateTimeOffset t0, T r);
	}
}

