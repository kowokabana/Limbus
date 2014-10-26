﻿using System;
using Limbus.Clockwork;
using System.Reactive;
using Limbus.Control;
using System.Collections.Generic;

namespace Limbus.Mosquito
{
	/// <summary>
	/// A thread-safe mosquito machine with a linear-behaving engine
	/// </summary>
	public class LinearMosquito : IControllable<double>, ITimed
	{
		public event Action<Timestamped<double>> Receive;

		public Timestamped<double> Setpoint { get; private set; }
		public TimeSpaned<double> Gradient { get; private set; }

		private object mutex = new object();
		private DateTimeOffset time = DateTimeOffset.UtcNow;
		private double actual = 0;

		public LinearMosquito (TimeSpaned<double> gradient)
		{
			if (gradient.Value <= 0) throw new ArgumentOutOfRangeException ("gradient");

			this.Setpoint = Timestamped.Create(0.0, DateTimeOffset.UtcNow);
			this.Gradient = gradient;
		}

		public void Send (Timestamped<double> setpoint)
		{
			lock (mutex)
			{
				this.actual = GetActual ();
				var earliestFinishTime = this.time.Add (actual.TimeTo(setpoint.Value, Gradient));

				this.Setpoint = setpoint.Timestamp > earliestFinishTime ?
					setpoint : Timestamped.Create(setpoint.Value, earliestFinishTime);
			}
		}

		private double GetActual()
		{
			if (time >= Setpoint.Timestamp) return Setpoint.Value;
			var engineStartTime = Setpoint.Timestamp - this.actual.TimeTo(Setpoint.Value, Gradient);
			if (time < engineStartTime) return 0.0;

			var timeSinceStart = time - engineStartTime;
			var delta = (timeSinceStart.TotalSeconds / Gradient.Duration.TotalSeconds) * Gradient.Value;
			return Setpoint.Value > this.actual ? this.actual + delta : this.actual - delta;
		}

		public void Set(DateTimeOffset time)
		{
			lock (mutex)
			{
				this.time = time;
				if (Receive != null) Receive(Timestamped.Create<double>(GetActual(), time));
			}
		}
	}
}

