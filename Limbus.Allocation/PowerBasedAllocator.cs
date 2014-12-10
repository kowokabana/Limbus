using System;
using System.Collections.Generic;
using System.Linq;
using Limbus.Log;
using Limbus.Things;

namespace Limbus.Allocation
{
	public class PowerBasedAllocator : IAllocator<double, Engine<double>>
	{
		public bool TryAllocate(IEnumerable<AllocatableThing<double, Engine<double>>> allocatables, double setpoint)
		{
			if (allocatables.Any(a => a.Thing.Variability == Variability.Staged))
				Logger.Log("PowerBasedAllocator", "Engines contain stepped engine that will be ignored", Level.Warning);

			var contiuousAllocatables = allocatables.Where(a => a.Thing.Variability == Variability.Continuously);
			var swarmPower = contiuousAllocatables.Sum(a => a.Thing.Stages.Max() - a.Thing.Stages.Min());

			foreach (var a in contiuousAllocatables) {
				var enginePower = a.Thing.Stages.Max() - a.Thing.Stages.Min();
				var weight = 1 / swarmPower * enginePower;
				a.Allocated = weight * setpoint;
			}

			return true;
		}
	}
}

