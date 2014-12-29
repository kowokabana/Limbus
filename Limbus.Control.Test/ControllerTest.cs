using System;
using NUnit.Framework;
using Limbus.Mosquito;
using Limbus.Clockwork;
using System.Linq;
using System.Collections.Generic;
using MoreLinq;
using Limbus.Allocation;
using Limbus.API;
using Limbus.Spec;

namespace Limbus.Control.Test
{
	[TestFixture]
	public class ControllerTest
	{
		[Test]
		public void Control_linear_mosquito_with_pid()
		{
			var receivedSeries = new List<Timestamped<double>>();
			var t0 = 0.AsMinute();
			var setpoint = 10.0.At(t0);//.Add(5.min()));
			var pid = new PIDAlgorithm(0.5, 0.5, 0.5, 0.0);

			var mosquito = new LinearMosquito(2.0.In(1.min()), t0);
			var controlledMosqito = mosquito.ControlledBy(pid);

			controlledMosqito.Receive += (t) => receivedSeries.Add(t);
			controlledMosqito.Send(setpoint);

			var clock = new Clock(t0);
			clock.Subscribe(mosquito);

			clock.Tick(5.min());
			//while (true) clock.Tick(1.min());
			Assert.AreEqual(setpoint, receivedSeries.Last());
		}

		[Test]
		public void Control_swarm_of_10_with_pid()
		{
			var receivedSeries = new List<Timestamped<double>>();
			var t0 = 0.AsMinute();
			var setpoint = 100.0.At(t0.Add(5.min()));

			var swarm = Enumerable.Range(0, 10).Select(i => new LinearMosquito(2.0.In(1.min()), t0)).ToList();
			// max.At(5.min)=10x10=100

			var specifiedSwarm = swarm.Select(m => m.WithSpec(new Engine<double>()
				{ Gradient = m.Gradient, Stages = new SortedSet<double>() {0.0, 100.0} }));

			var swarmController = new SwarmController(
				specifiedSwarm, new PowerBasedAllocator(), new PIDAlgorithm(0.5, 0.5, 0.5, 0.0));

			swarmController.Receive += (t) => receivedSeries.Add(t);
			swarmController.Send(setpoint);

			var clock = new Clock(t0);
			swarm.ForEach(m => clock.Subscribe(m));

			clock.Tick(5.min());
			Assert.AreEqual(setpoint, receivedSeries.Last());
		}
	}
}

