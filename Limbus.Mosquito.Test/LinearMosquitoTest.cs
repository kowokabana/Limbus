using System;
using NUnit.Framework;
using Limbus.Clockwork;
using Limbus.Control;
using System.Collections.Generic;
using System.Linq;

namespace Limbus.Mosquito.Test
{
	[TestFixture]
	public class LinearMosquitoTest
	{
		private DateTimeOffset t0;
		private LinearMosquito mock;
		private List<Timestamped<double>> received;

		[SetUp]
		public void Setup()
		{
			t0 = 0.AsMinute();
			mock = new LinearMosquito(2.0.In(1.min()), t0);

			received = new List<Timestamped<double>>();
			mock.Receive += (v) => {
				received.Add(v);
			};
		}

		[Test]
		public void Send_setpoint()
		{
			mock.Send(20.0.At(t0.Add(10.min())));
			mock.Set(t0.Add(5.min()));
			mock.Set(t0.Add(10.min()));

			Assert.AreEqual(10.0.At(t0.Add(5.min())), received.First());
			Assert.AreEqual(20.0.At(t0.Add(10.min())), received.ElementAt(1));
		}

		[Test]
		public void Change_existing_setpoint()
		{
			mock.Send(20.0.At(t0.Add(10.min())));
			mock.Set(t0.Add(5.min()));
			mock.Send(10.0.At(t0.Add(10.min())));
			mock.Set(t0.Add(10.min()));

			Assert.AreEqual(10.0.At(t0.Add(5.min())), received.First());
			Assert.AreEqual(10.0.At(t0.Add(10.min())), received.ElementAt(1));
		}

		[Test]
		public void Send_setpoint_with_delay()
		{
			var delayedMock = mock.WithDelay(5.min());
			delayedMock.Send(20.0.At(t0.Add(10.min())).At(t0));
			// 20.0 at t0+10min not possible, because of delay, so 20.0 at t0+10min+5min

			for (int i = 0; i <= 15; i++) {
				var t1 = t0.Add(i.min());
				delayedMock.Set(t1);
				mock.Set(t1);
			}

			Assert.AreEqual(0.0.At(t0.Add(0.min())), received.ElementAt(0));
			Assert.AreEqual(0.0.At(t0.Add(5.min())), received.ElementAt(5));
			Assert.AreEqual(2.0.At(t0.Add(6.min())), received.ElementAt(6));
			Assert.AreEqual(10.0.At(t0.Add(10.min())), received.ElementAt(10));
			Assert.AreEqual(20.0.At(t0.Add(15.min())), received.ElementAt(15));
		}
	}
}

