using System;
using Limbus.Clockwork;
using Limbus.API;

namespace Limbus.API
{
	public class ControlEntity<T, T2> : IControllable<T>, IAllocatable<T>, ISpecified<T2>, ISpecifiedAllocatable<T, T2>
	{
		private IControllable<T> Controllable;
		public event Action<Timestamped<T>> Receive
		{
			add
			{
				this.Controllable.Receive += value;
			}
			remove
			{
				this.Controllable.Receive -= value;
			}
		}

		public void Send(Timestamped<T> setpoint) { this.Controllable.Send(setpoint); }
		public T2 Spec { get; private set; }

		public ControlEntity(IControllable<T> controllable, T2 spec)
		{
			this.Controllable = controllable;
			this.Spec = spec;
		}

		public void Allocate(T quantity) { Quantity = quantity; }
		public T Quantity { get; private set; }

		public override string ToString()
		{
			return string.Format("[ControlEntity: Spec={0}, Quantity={1}]", Spec, Quantity);
		}
	}

	public static class ControlEntityEx
	{
		public static ControlEntity<T, T2> WithSpec<T, T2>(this IControllable<T> controllable, T2 spec)
		{
			return new ControlEntity<T, T2>(controllable, spec);
		}
	}
}

