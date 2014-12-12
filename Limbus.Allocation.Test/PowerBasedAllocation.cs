using System;
using NUnit.Framework;
using System.Collections.Generic;
using Limbus.Specs;

namespace Limbus.Allocation.Test
{
	[TestFixture]
	public class PowerBasedAllocation
	{
		[Test]
		public void Allocate_setpoint_to_swarm()
		{
			var engine100 = new Engine<double>() { Stages = new SortedSet<double>() { 0.0, 100 } }.AsAllocatable();
			var engine200 = new Engine<double>() { Stages = new SortedSet<double>() { 0.0, 200 } }.AsAllocatable();
			var engine300 = new Engine<double>() { Stages = new SortedSet<double>() { 0.0, 300 } }.AsAllocatable();
			var engineSkipped = new Engine<double>() { Stages = new SortedSet<double>() { 0.0, 300 }, Variability = Variability.Staged }.AsAllocatable();
			var allocator = new PowerBasedAllocator();
			var success = allocator.TryAllocate(new[] { engine100, engine200, engine300, engineSkipped }, 600);

			Assert.IsTrue(success);
			Assert.AreEqual(100, engine100.Quantity, 0.01);
			Assert.AreEqual(200, engine200.Quantity, 0.01);
			Assert.AreEqual(300, engine300.Quantity, 0.01);
			Assert.AreEqual(0, engineSkipped.Quantity, 0.01);
		}
	}
}

