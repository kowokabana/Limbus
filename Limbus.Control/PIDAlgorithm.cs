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
	/// </summary>
	public class PIDAlgorithm : IControlAlgorithm<double>
	{
		private double r = 0;
		private double integral = 0;
		private DateTimeOffset t0 = 0.AsMinute();
		private double e0 = 0;

		/// <summary>
		/// Proportional gain
		/// </summary>
		/// <value>Proportional gain</value>
		public double kP { get; set; }

		/// <summary>
		/// Sums up the error of the past
		/// </summary>
		/// <value>Integral gain</value>
		public double kI { get; set; }

		/// <summary>
		/// Extrapolates the future error lineary 
		/// </summary>
		/// <value>Differential gain</value>
		public double kD { get; set; }

		/// <summary>
		/// Feedforward term of the process
		/// </summary>
		/// <value>Feedforward gain</value>
		public double kR { get; set; }

		public PIDAlgorithm(double kP, double kI, double kD, double kR)
		{
			this.kP = kP;
			this.kI = kI;
			this.kD = kD;
			this.kR = kR;
		}

		public bool TryControl(Timestamped<double> y, out Timestamped<double> u)
		{
			u = y; // this u is never used outside
			var t = y.Timestamp;

			var dt = (t - t0).TotalMilliseconds * 0.00001; // time past since last add to integral
			if (dt == 0) return false;
			var e = this.r - y.Value;
			var de = e - e0;
			this.integral += e * dt;
			var derivative = de / dt;
			u = (kP * e + kI * integral + kD * derivative + kR * this.r).At(t);

			e0 = e;
			t0 = t;

			return true;
		}

		public void Reset(Timestamped<double> r)
		{
			this.integral = 0;
			this.e0 = 0;
			this.t0 = r.Timestamp;
			this.r = r.Value;
		}
	}
}

