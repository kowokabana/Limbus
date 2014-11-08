using System;
using Limbus.Clockwork;

namespace Limbus.Control
{
	public interface IEngine<T>
	{
		TimeSpaned<T> Gradient { get; }
		TimeSpan Deadtime { get; }
	}
}

