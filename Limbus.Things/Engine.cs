using System;
using Limbus.Clockwork;
using System.Collections.Generic;

namespace Limbus.Things
{
	public class Engine<T>
	{
		public TimeSpan Deadtime { get; set;}
		public TimeSpaned<T> Gradient { get; set;}
		public SortedSet<T> Stages { get; set;}
		public Variability Variability { get; set;}

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

