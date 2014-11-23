using System;
using NUnit.Framework;
using Limbus.Mosquito;
using Limbus.Clockwork;
using System.Linq;

namespace Limbus.Control.Test
{
	[TestFixture]
	public class PIDControllerTest
	{
		[Test]	
		public void Distribute_setpoint_to_swarm()
		{
			var tStart = 0.AsMinute();
			var swarm = Enumerable.Range(0, 9).Select(i => new LinearMosquito(2.0.In(1.min()), tStart));
			var pid = new PIDController(swarm);
			pid.Send(10.At(tStart.Add(5.min)));
		}
	}
}

