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
		private DateTimeOffset tStart;
		private LinearMosquito mock;
		private List<Timestamped<double>> received;

		[SetUp]
		public void Setup()
		{
			tStart = 0.AsMinute();
			mock = new LinearMosquito(2.0.In(1.min()), 0.min(), tStart);

			received = new List<Timestamped<double>>();
			mock.Receive += (v) => {
				received.Add(v);
			};
		}

		[Test]
		public void Send_setpoint()
		{
			mock.Send(20.0.At(tStart.Add(10.min())));
			mock.Set(tStart.Add(5.min()));
			mock.Set(tStart.Add(10.min()));

			Assert.AreEqual(10.0.At(tStart.Add(5.min())), received.First());
			Assert.AreEqual(20.0.At(tStart.Add(10.min())), received.ElementAt(1));
		}

		[Test]
		public void Change_existing_setpoint()
		{
			mock.Send(20.0.At(tStart.Add(10.min())));
			mock.Set(tStart.Add(5.min()));
			mock.Send(10.0.At(tStart.Add(10.min())));
			mock.Set(tStart.Add(10.min()));

			Assert.AreEqual(10.0.At(tStart.Add(5.min())), received.First());
			Assert.AreEqual(10.0.At(tStart.Add(10.min())), received.ElementAt(1));
		}

		[Test]
		public void Send_with_deadtime()
		{
			mock.Send(20.0.At(tStart.Add(15.min())));
			mock.Set(tStart.Add(5.min()));
			mock.Set(tStart.Add(10.min()));
			mock.Set(tStart.Add(15.min()));

			Assert.AreEqual(0.0.At(tStart.Add(5.min())), received.First());
			Assert.AreEqual(10.0.At(tStart.Add(10.min())), received.ElementAt(1));
			Assert.AreEqual(20.0.At(tStart.Add(15.min())), received.ElementAt(2));
		}
	}
}

