using System;
using System.Collections.Generic;
using Limbus.Clockwork;
using System.Linq;
using MoreLinq;
using Limbus.Linq;
using Limbus.Allocation;
using Limbus.API;
using Limbus.Spec;

namespace Limbus.Control
{
	/// <summary>
	/// PID controller
	///
	/// Variables
	/// 
	/// r - setpoint
	/// u - calculated process input variable
	/// y - process output variable (actual)
	/// e - error = r-y
	/// 
	/// Algorithm
	/// 
	/// previous_error = 0
	/// integral = 0
	///	start:
	///	error = setpoint - measured_value
	///	integral = integral + error*dt
	///	derivative = (error - previous_error)/dt
	///	output = Kp*error + Ki*integral + Kd*derivative
	///	previous_error = error
	///	wait(dt)
	///	goto start
	/// 
	/// </summary>
	public class PIDController : IController<double, Engine<double>>, IControllable<double>
	{
		private const double kP = 5;
		private const double kI = 3;
		private const double kD = 0.01;

		private double r = 0;
		private double integral = 0;
		private DateTimeOffset t0;
		private double e0 = 0;

		private object mutex = new object();
		private Dictionary<ControlEntity<double, Engine<double>>, Control.Pair> controlledSwarm;
		private IAllocator<double, Engine<double>> allocator;

		public void Update(IEnumerable<ControlEntity<double, Engine<double>>> swarm, IAllocator<double, Engine<double>> allocator)
		{
			lock (mutex) {
				this.allocator = allocator;
				this.controlledSwarm = new Dictionary<ControlEntity<double, Engine<double>>, Control.Pair>();
				foreach (var c in swarm) {
					this.controlledSwarm.Add(c, new Control.Pair(0.0.At(0.AsMinute()), 0.0.At(0.AsMinute()))); // TODO: make 0.0 to null
					c.Receive += (t) => OnReceive(c, t);
				}
			}
		}

		public PIDController(IEnumerable<ControlEntity<double, Engine<double>>> swarm, IAllocator<double, Engine<double>> allocator)
		{
			Update(swarm, allocator);
		}

		private void OnReceive(ControlEntity<double, Engine<double>> sender, Timestamped<double> actual)
		{
			lock (mutex) {
				controlledSwarm[sender].Actual = actual;
				var y = controlledSwarm.Values.Sum(pair => pair.Actual.Value);
				Control(actual.Timestamp, y);
				if (Receive != null)
					Receive(y.At(controlledSwarm.Values.Max(a => a.Actual.Timestamp)));
			}
		}

		private void Control(DateTimeOffset t, double y)
		{
			var dt = (t - t0).TotalMilliseconds * 0.0000001; // time past since last add to integral
			if (dt == 0) return;

			var e = r - y;
			this.integral += e * dt;
			var derivative = (e - e0) / dt;
			var u = kP * e + kI * integral + kD * derivative;

			t0 = t;
			e0 = e;

			Send(u.At(t), false);
		}

		public event Action<Timestamped<double>> Receive;

		public void Send(Timestamped<double> setpoint)
		{
			Send(setpoint, true);
		}

		private void Send(Timestamped<double> setpoint, bool reset)
		{
			lock (mutex) {
				if (reset) {
					t0 = setpoint.Timestamp;
					r = setpoint.Value;
				}
				if(!allocator.TryAllocate(controlledSwarm.Keys, setpoint.Value)) return;
				foreach (var c in controlledSwarm.Keys) {
					var partialSetpoint = c.Quantity.At(setpoint.Timestamp);
					controlledSwarm[c].Setpoint = partialSetpoint;
					c.Send(partialSetpoint);
				}
			}
		}
	}
}

