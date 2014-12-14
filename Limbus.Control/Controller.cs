using System;
using Limbus.API;
using Limbus.Clockwork;

namespace Limbus.Control
{
	public class Controller : IController<double>, IControllable<double>
	{
		private IControlAlgorithm<double> algorithm;

		public Controller(IControlAlgorithm<double> algorithm)
		{
			Update(algorithm);
		}

		public void Update(IControlAlgorithm<double> algorithm)
		{
			this.algorithm = algorithm;
		}
			
		public event Action<Timestamped<double>> Receive;

		public void Send(Timestamped<double> setpoint)
		{
			algorithm.Reset(setpoint);
			if (Receive != null) Receive(setpoint);
		}

		public void Input(Timestamped<double> y)
		{
			Timestamped<double> u;
			if (algorithm.TryControl(y, out u)) Receive(u);
		}
	}

	public static class ControllerEx
	{
		public static Controller ControlledBy(this IControllable<double> process,
			IControlAlgorithm<double> algorithm)
		{
			var controller = new Controller(algorithm);
			process.Receive += controller.Input;
			controller.Receive += process.Send;
			return controller;
		}
	}
}

