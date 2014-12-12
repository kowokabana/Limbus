using System;
using System.Collections.Generic;
using Limbus.Clockwork;
using System.Linq;
using MoreLinq;
using Limbus.Linq;
using Limbus.Allocation;
using Limbus.API;
using Limbus.Spec;

namespace Limbus.Control
{
	public class PIDController : IController<double, Engine<double>>, IControllable<double>
	{
		private object mutex = new object();
		private Dictionary<ControlEntity<double, Engine<double>>, Control.Pair> controlledSwarm;
		private IAllocator<double, Engine<double>> allocator;

		public void Update(IEnumerable<ControlEntity<double, Engine<double>>> swarm, IAllocator<double, Engine<double>> allocator)
		{
			lock (mutex) {
				this.allocator = allocator;
				this.controlledSwarm = new Dictionary<ControlEntity<double, Engine<double>>, Control.Pair>();
				foreach (var c in swarm) {
					this.controlledSwarm.Add(c, new Control.Pair(0.0.At(0.AsMinute()), 0.0.At(0.AsMinute()))); // TODO: make 0.0 to null
					c.Receive += (t) => OnReceive(c, t);
				}
			}
		}

		public PIDController(IEnumerable<ControlEntity<double, Engine<double>>> swarm, IAllocator<double, Engine<double>> allocator)
		{
			Update(swarm, allocator);
		}

		private void OnReceive(ControlEntity<double, Engine<double>> sender, Timestamped<double> actual)
		{
			lock (mutex) {
				controlledSwarm[sender].Actual = actual;
				if (Receive != null)
					Receive(controlledSwarm.Values.Sum(pair => pair.Actual.Value).At(controlledSwarm.Values.Max(a => a.Actual.Timestamp)));
			}
		}

		public event Action<Timestamped<double>> Receive;

		public void Send(Timestamped<double> setpoint)
		{
			lock (mutex) {
				if(!allocator.TryAllocate(controlledSwarm.Keys, setpoint.Value)) return;
				foreach (var c in controlledSwarm.Keys) {
					var partialSetpoint = c.Quantity.At(setpoint.Timestamp);
					controlledSwarm[c].Setpoint = partialSetpoint;
					c.Send(partialSetpoint);
				}
			}
		}
	}
}

