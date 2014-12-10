using System;
using System.Collections.Generic;
using System.Linq;
using Limbus.Log;

namespace Limbus.Allocation
{
	public class PowerBasedAllocator : IAllocator<double, Engine<double>>
	{
		public bool TryAllocate(IEnumerable<Allocatable<double, Engine<double>>> allocatables, double setpoint)
		{
			if (allocatables.Any(a => a.Parameters.Variability == Variability.Staged))
				Logger.Log("PowerBasedAllocator", "Engines contain stepped engine that will be ignored", Level.Warning);

			var contiuousAllocatables = allocatables.Where(a => a.Parameters.Variability == Variability.Continuously);
			var swarmPower = contiuousAllocatables.Sum(a => a.Parameters.Stages.Max() - a.Parameters.Stages.Min());

			foreach (var a in contiuousAllocatables) {
				var enginePower = a.Parameters.Stages.Max() - a.Parameters.Stages.Min();
				var weight = 1 / swarmPower * enginePower;
				a.Allocated = weight * setpoint;
			}

			return true;
		}
	}
}

