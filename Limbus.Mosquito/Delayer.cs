using System;
using Limbus.Control;
using Limbus.Clockwork;
using System.Collections.Generic;
using System.Linq;

namespace Limbus.Mosquito
{
	public class Delayer<T> : IControllable<Timestamped<T>>, ITimed
	{
		private TimeSpan delay;
		private Queue<Timestamped<Timestamped<T>>> setpointQueue;
		private object mutex = new object();

		public Delayer(TimeSpan delay)
		{
			if (delay < TimeSpan.Zero) throw new ArgumentException("delay must be positive");
			this.delay = delay;
			this.setpointQueue = new Queue<Timestamped<Timestamped<T>>>();
		}

		public void Send(Timestamped<Timestamped<T>> setpoint)
		{
			lock (mutex) {
				this.setpointQueue.Enqueue(setpoint);
			}
		}

		public void Set(DateTimeOffset time)
		{
			if (Receive == null) return;
			if (!this.setpointQueue.Any()) return;
			lock (mutex) {
				var first = this.setpointQueue.Peek();
				if (time > first.Timestamp.Add(this.delay)) {
					Receive(this.setpointQueue.Dequeue());
				}
			}
		}

		public event Action<Timestamped<Timestamped<T>>> Receive;
	}

	public static class DelayerEx
	{
		public static Delayer<T> WithDelay<T>(this IControllable<T> controllable, TimeSpan delay)
		{
			var delayer = new Delayer<T>(delay);
			delayer.Receive += 
				(Timestamped<Timestamped<T>> delayedSetpoint) => controllable.Send(delayedSetpoint.Value);
			return delayer;
		}
	}
}

