using System;
using Limbus.Things;
using Limbus.Clockwork;
using Limbus.API;

namespace Limbus.Control
{
	public class ControllableThing<T, T2> : IControllable<T>
	{
		private IControllable<T> Controllable;
		public event Action<Timestamped<T>> Receive
		{
			add {this.Controllable.Receive += value;} //TODO: add lock here
			remove {this.Controllable.Receive -= value;} //TODO: add lock here
		}
		public void Send(Timestamped<T> setpoint) { Controllable.Send(setpoint); }

		public T2 Thing { get; private set;}

		public ControllableThing(IControllable<T> controllable, T2 thing)
		{
			this.Thing = thing;
			this.Controllable = controllable;
		}
	}
}

