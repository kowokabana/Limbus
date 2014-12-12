using System;
using System.Collections.Generic;
using Limbus.Clockwork;
using Limbus.API;

namespace Limbus.Spec
{
	public class Engine<T>
	{
		public TimeSpan Deadtime { get; set; }
		public TimeSpaned<T> Gradient { get; set; }
		public SortedSet<T> Stages { get; set; }
		public Variability Variability { get; set; }

		public Engine()
		{
			Deadtime = 0.0.s();
			Gradient = TimeSpaned.Create<T>(default(T), 0.0.s());
			Stages = new SortedSet<T>();
			Variability = Variability.Continuously;
		}
	}

	public static class EngineEx
	{
		public static SpecifiedAllocatable<T, Engine<T>> AsAllocatable<T>(this Engine<T> engine) 
		{
			return new SpecifiedAllocatable<T, Engine<T>>(engine);
		}
	}

	public enum Variability
	{
		Continuously,
		Staged
	}
}

