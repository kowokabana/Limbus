using System;
using Limbus.Things;

namespace Limbus.Allocation
{
	public class AllocatableThing<T, T2>
	{
		public T Allocated { get; set; }
		public T2 Thing { get; private set;}

		public AllocatableThing(T2 thing)
		{
			Thing = thing;
		}
	}

	public static class EngineEx
	{
		public static AllocatableThing<T, Engine<T>> AsAllocatable<T>(this Engine<T> engine)
		{
			return new AllocatableThing<T, Engine<T>>(engine);
		}
	}
}

