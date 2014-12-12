using System;
using System.Collections.Generic;
using System.Linq;
using Limbus.Log;
using Limbus.API;
using Limbus.Specs;

namespace Limbus.Allocation
{
	public class PowerBasedAllocator : IAllocator<double, Engine<double>>
	{
		public bool TryAllocate(IEnumerable<ISpecifiedAllocatable<double, Engine<double>>> allocatables, double setpoint)
		{
			if (allocatables.Any(a => a.Spec.Variability == Variability.Staged))
				Logger.Log("PowerBasedAllocator", "Engines contain stepped engine that will be ignored", Level.Warning);

			var contiuousAllocatables = allocatables.Where(a => a.Spec.Variability == Variability.Continuously);
			var swarmPower = contiuousAllocatables.Sum(a => a.Spec.Stages.Max() - a.Spec.Stages.Min());

			foreach (var a in contiuousAllocatables) {
				var enginePower = a.Spec.Stages.Max() - a.Spec.Stages.Min();
				var weight = 1 / swarmPower * enginePower;
				a.Allocate(weight * setpoint);
			}

			return true;
		}
	}
}

