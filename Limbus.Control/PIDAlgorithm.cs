using System;
using Limbus.API;
using Limbus.Clockwork;

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
	public class PIDAlgorithm : IControlAlgorithm<double>
	{
		private double r = 0;
		private double integral = 0;
		private DateTimeOffset t0 = 0.AsMinute();
		private double e0 = 0;

		public double kP { get; set; }
		public double kI { get; set; }
		public double kD { get; set; }

		public PIDAlgorithm(double kP, double kI, double kD)
		{
			this.kP = kP;
			this.kI = kI;
			this.kD = kD;
		}

		public bool TryControl(DateTimeOffset t, double y, out double u)
		{
			u = y; // this u is never used outside

			var dt = (t - t0).TotalMilliseconds * 0.00001; // time past since last add to integral
			if (dt == 0) return false; //todo: check this

			var e = this.r - y;
			this.integral += e * dt;
			var derivative = (e - e0) / dt;
			u = kP * e + kI * integral; //+ kD * derivative;

			e0 = e;
			t0 = t;

			return true;
		}

		public void Reset(DateTimeOffset t0, double r)
		{
			this.t0 = t0;
			this.r = r;
		}
	}
}

