using System;
using Limbus.Clockwork;

namespace Limbus.API
{
	public interface IControlAlgorithm<T>
	{
		bool TryControl(Timestamped<T> y, out Timestamped<T> u);
		void Reset(Timestamped<T> r);
	}
}

