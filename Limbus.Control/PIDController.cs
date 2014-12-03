using System;
using System.Collections.Generic;
using Limbus.Clockwork;
using System.Linq;
using MoreLinq;
using Limbus.Linq;

namespace Limbus.Control
{
	public class PIDController : IController<double>, IControllable<double>
	{
		private object mutex = new object();
		private Dictionary<IControllable<double>, Control.Pair> swarm;

		public PIDController(IEnumerable<IControllable<double>> swarm)
		{
			Update(swarm);
		}

		private void OnReceive(IControllable<double> sender, Timestamped<double> actual)
		{
			lock (mutex) {
				swarm[sender].Actual = actual;
				if (Receive != null)
					Receive(swarm.Values.Sum(pair => pair.Actual.Value).At(swarm.Values.Max(a => a.Actual.Timestamp)));
			}
		}

		public event Action<Timestamped<double>> Receive;

		public void Send(Timestamped<double> setpoint)
		{
			lock (mutex) {
				//TODO: must be splitted according to rate of machine power compared to pool power
				var splittedSetpoint = setpoint.Value / this.swarm.Count();

				foreach (var c in swarm) {
					c.Value.Setpoint = splittedSetpoint.At(setpoint.Timestamp);
					c.Key.Send(splittedSetpoint.At(setpoint.Timestamp));
				}
			}
		}

		public void Update(IEnumerable<IControllable<double>> swarm)
		{
			lock (mutex) {
				this.swarm = new Dictionary<IControllable<double>, Control.Pair>();
				foreach (var c in swarm) {
					this.swarm.Add(c, new Control.Pair(0.0.At(0.AsMinute()), 0.0.At(0.AsMinute())));
					c.Receive += (t) => OnReceive(c, t);
				}
			}
		}
	}
}

