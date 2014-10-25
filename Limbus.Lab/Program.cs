using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Gtk;
using Limbus.Clockwork;
using Limbus.Mosquito;
using Limbus.Plotter;
using OxyPlot;
using OxyPlot.Axes;

namespace Limbus.Lab
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();

			MainWindow win = new MainWindow ();

			var tMin = new DateTimeOffset (1, 1, 1, 0, 10, 0, TimeSpan.Zero);
			var tMax = new DateTimeOffset (1, 1, 1, 0, 25, 0, TimeSpan.Zero);

			var timePlot = new TimePlot ("Mosquito Population", tMin, tMax, 0, 30);

			var clock = new Clock (tMin);
			var mock = new LinearMosquito (new TimeSpaned<double>(2, 1.min()));
			clock.Subscribe (mock);

			mock.Receive += (ts) => {
				timePlot.Line.Points.Add (DateTimeAxis.CreateDataPoint (ts.Timestamp.DateTime, ts.Value));
				timePlot.InvalidatePlot(true);
			};

			mock.Send(Timestamped.Create<double>(20, tMax));

			Task.Run (() => {
				for (int i = 0; i <= 20; i++) { // t = 10 -> t = 30
					clock.Tick (TimeSpan.FromMinutes (1));
					Thread.Sleep (500);
				}
			});

			var plotView = new OxyPlot.GtkSharp.PlotView { Model = timePlot };

			plotView.SetSizeRequest(400, 400);
			plotView.Visible = true;

			win.SetSizeRequest(600, 600);
			win.Add(plotView);

			win.Show ();
			Application.Run ();
		}
	}
}
