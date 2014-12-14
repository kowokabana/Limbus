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
	public class SwarmController : ISwarmController<double, Engine<double>>, IControllable<double>
	{
		private object mutex = new object();
		private Dictionary<ControlEntity<double, Engine<double>>, Control.Pair> controlledSwarm;
		private IAllocator<double, Engine<double>> allocator;
		private IControlAlgorithm<double> algorithm;

		public SwarmController(
			IEnumerable<ControlEntity<double, Engine<double>>> swarm,
			IAllocator<double, Engine<double>> allocator,
			IControlAlgorithm<double> algorithm)
		{
			Update(swarm, allocator, algorithm);
		}

		public void Update(
			IEnumerable<ControlEntity<double, Engine<double>>> swarm,
			IAllocator<double, Engine<double>> allocator,
			IControlAlgorithm<double> algorithm)
		{
			lock (mutex) {
				this.allocator = allocator;
				this.algorithm = algorithm;
				this.controlledSwarm = new Dictionary<ControlEntity<double, Engine<double>>, Control.Pair>();
				foreach (var c in swarm) {
					this.controlledSwarm.Add(c, new Control.Pair(0.0.At(0.AsMinute()), 0.0.At(0.AsMinute()))); // TODO: make 0.0 to null
					c.Receive += (t) => OnReceive(c, t);
				}
			}
		}

		private void OnReceive(ControlEntity<double, Engine<double>> sender, Timestamped<double> actual)
		{
			lock (mutex) {
				controlledSwarm[sender].Actual = actual;

				var y = controlledSwarm.Values.Sum(pair => pair.Actual.Value)
								.At(controlledSwarm.Values.Max(pair => pair.Actual.Timestamp)); //todo: check if neccessary, or if actual.Timestamp is enough
				Timestamped<double> u;
				if(algorithm.TryControl(y, out u)) Send(u, false);

				if (Receive != null) Receive(y); 
			}
		}

		public event Action<Timestamped<double>> Receive;

		public void Send(Timestamped<double> setpoint)
		{
			Send(setpoint, true);
		}

		private void Send(Timestamped<double> setpoint, bool reset)
		{
			lock (mutex) {
				if (reset) algorithm.Reset(setpoint);
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

