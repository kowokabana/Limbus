using System;
using NUnit.Framework;
using Limbus.Mosquito;
using Limbus.Clockwork;
using System.Linq;
using System.Collections.Generic;
using MoreLinq;
using Limbus.Allocation;
using Limbus.API;
using Limbus.Specs;

namespace Limbus.Control.Test
{
	[TestFixture]
	public class PIDControllerTest
	{
		[Test]
		public void Distribute_setpoint_to_swarm_of_10()
		{
			var receivedSeries = new List<Timestamped<double>>();
			var t0 = 0.AsMinute();
			var setpoint = 100.0.At(t0.Add(5.min()));

			var swarm = Enumerable.Range(0, 10).Select(i => new LinearMosquito(2.0.In(1.min()), t0)).ToList();
			// max.At(5.min)=10x10=100

			var specifiedSwarm = swarm.Select(m => m.WithSpec(new Specs.Engine<double>()
				{ Gradient = m.Gradient, Stages = new SortedSet<double>() {0.0, 100.0} }));

			var pid = new PIDController(specifiedSwarm, new PowerBasedAllocator());
			pid.Receive += (t) => receivedSeries.Add(t);
			pid.Send(setpoint);

			var clock = new Clock(t0);
			swarm.ForEach(m => clock.Subscribe(m));

			clock.Tick(5.min());
			Assert.AreEqual(setpoint, receivedSeries.Last());
		}
	}
}

