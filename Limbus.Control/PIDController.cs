using System;
using System.Collections.Generic;
using Limbus.Clockwork;
using System.Linq;
using MoreLinq;
using Limbus.Linq;
using Limbus.Allocation;
using Limbus.Things;
using Limbus.API;

namespace Limbus.Control
{
	public class PIDController : IController<double, Engine<double>>, IControllable<double>
	{
		private object mutex = new object();
		private Dictionary<ControllableThing<double, Engine<double>>, Control.Pair> controlledSwarm;
		private IAllocator<double, Engine<double>> allocator;

		public void Update(IEnumerable<ControllableThing<double, Engine<double>>> swarm, IAllocator<double, Engine<double>> allocator)
		{
			lock (mutex) {
				this.allocator = allocator;
				this.controlledSwarm = new Dictionary<ControllableThing<double, Engine<double>>, Control.Pair>();
				foreach (var c in swarm) {
					this.controlledSwarm.Add(c, new Control.Pair(0.0.At(0.AsMinute()), 0.0.At(0.AsMinute()))); // TODO: make 0.0 to null
					c.Receive += (t) => OnReceive(c, t);
				}
			}
		}

		public PIDController(IEnumerable<ControllableThing<double, Engine<double>>> swarm, IAllocator<double, Engine<double>> allocator)
		{
			Update(swarm, allocator);
		}

		private void OnReceive(ControllableThing<double, Engine<double>> sender, Timestamped<double> actual)
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
				//allocator.TryAllocate(

				//TODO: must be splitted according to rate of machine power compared to pool power
				//TODO: allocator.TryAllocate(swarm.Values);
				var splittedSetpoint = setpoint.Value / this.controlledSwarm.Count();

				foreach (var c in controlledSwarm) {
					c.Value.Setpoint = splittedSetpoint.At(setpoint.Timestamp);
					c.Key.Send(splittedSetpoint.At(setpoint.Timestamp));
				}
			}
		}
	}
}

