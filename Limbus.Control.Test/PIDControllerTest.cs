using System;
using NUnit.Framework;
using Limbus.Mosquito;
using Limbus.Clockwork;
using System.Linq;
using System.Collections.Generic;
using MoreLinq;

namespace Limbus.Control.Test
{
	[TestFixture]
	public class PIDControllerTest
	{
		[Test]
		public void Distribute_setpoint_to_swarm_of_10()
		{
			var receivedSeries = new List<Timestamped<double>>();
			var tStart = 0.AsMinute();
			var setpoint = 100.0.At(tStart.Add(5.min()));

			var swarm = Enumerable.Range(0, 10).Select(i => new LinearMosquito(2.0.In(1.min()), tStart)).ToList();
			// max.At(5.min)=10x10=100

			var pid = new PIDController(swarm);
			pid.Receive += (t) => receivedSeries.Add(t);
			pid.Send(setpoint);

			var clock = new Clock(tStart);
			swarm.ForEach(m => clock.Subscribe(m));

			clock.Tick(5.min());
			Assert.AreEqual(setpoint, receivedSeries.Last());
		}
	}
}

