using System;
using Limbus.Clockwork;

namespace Limbus.Control
{
	public interface IControllable<T>
	{
		void Send(Timestamped<T> setpoint);
		event Action<Timestamped<T>> Receive;
	}
}

