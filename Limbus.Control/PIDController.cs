using System;
using System.Collections.Generic;
using Limbus.Clockwork;
using System.Linq;
using MoreLinq;

namespace Limbus.Control
{
	public class PIDController : IController<double>
	{
		private IEnumerable<IControllable<double>> swarm;
		private object mutex = new object();
		private Dictionary<IControllable<double>, Timestamped<double>> actuals;

		public PIDController(IEnumerable<IControllable<double>> swarm)
		{
			this.swarm = swarm;
			this.actuals = new Dictionary<IControllable<double>, Timestamped<double>>();
   		this.swarm.ForEach(c => c.Receive += (t) => OnReceive(c, t));
		}

		private void OnReceive(IControllable<double> sender, Timestamped<double> actual)
		{
			lock (mutex) {
				actuals.Add(sender, actual);
				if (Receive != null)
					Receive(actuals.Values.Sum(a => a.Value).At(actuals.Values.Max(a => a.Timestamp)));
			}
		}

		public event Action<Timestamped<double>> Receive;

		public void Send(Timestamped<double> setpoint)
		{
			lock (mutex) {
				var splittedSetpoint = setpoint.Value / this.swarm.Count();
				this.swarm.ForEach(c => c.Send(splittedSetpoint.At(setpoint.Timestamp)));
			}
		}

		public void Update(IEnumerable<IControllable<double>> swarm)
		{
			lock (mutex) {
				this.swarm = swarm;
			}
		}
	}
}

