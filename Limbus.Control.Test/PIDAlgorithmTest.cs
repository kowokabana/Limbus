using System;
using NUnit.Framework;
using Limbus.Clockwork;

namespace Limbus.Control.Test
{
	[TestFixture]
	public class PIDAlgorithmTest
	{
		[Test]
		public void Pump_some_value_to_pid()
		{
			var t0 = 0.AsMinute();

			var pid = new PIDAlgorithm(0.5, 0.5, 0.5, 0.0);
			pid.Reset(10.0.At(t0));
			Timestamped<double> u;
			var success = pid.TryControl(8.0.At(t0.Add(5.min())), out u);

			Assert.IsTrue(success);
			Assert.AreEqual(4.At(t0.Add(5.min())), u);
		}
	}
}

