using System;

namespace Limbus.API
{
	public interface IController<T>
	{
		void Update(IControlAlgorithm<T> algorithm);
	}
}

