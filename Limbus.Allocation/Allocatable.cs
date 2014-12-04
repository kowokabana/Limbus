using System;

namespace Limbus.Allocation
{
	public class Allocatable<T1, T2>
	{
		public T1 Allocated { get; set; }
		public T2 Parameters { get; private set;}

		public Allocatable(T2 parameters)
		{
			Parameters = parameters;
		}
	}
}

