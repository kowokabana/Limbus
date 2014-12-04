using System;
using Limbus.Clockwork;
using System.Collections.Generic;

namespace Limbus.Allocation
{
	public class Engine<T>
	{
		public TimeSpan Deadtime { get; private set;}
		public TimeSpaned<T> Gradient { get; private set;}
		public SortedSet<T> Stages { get; private set;}
		public Variability Variability { get; private set;}

		public Engine()
		{
			Deadtime = 0.0.s();
			Gradient = TimeSpaned.Create<T>(default(T), 0.0.s());
			Stages = new SortedSet<T>();
			Variability = Variability.Continuously;
		}
	}

	public enum Variability
	{
		Continuously,
		Staged
	}
}

