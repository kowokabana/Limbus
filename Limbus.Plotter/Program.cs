using System;
using Gtk;
using OxyPlot;
using OxyPlot.Series;
using Limbus.Control;
using Limbus.Clockwork;
using Limbus.Mosquito;
using System.Reactive;
using OxyPlot.Axes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Limbus.Plotter
{
	public class Item
	{
		public DateTime X { get; set; }
		public double Y { get; set; }
	}

	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();

			var tmp = new PlotModel { Title = "Test" };
			tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Left, 
				MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, TickStyle = TickStyle.Outside });
			//var dt = DateTime.Now;
			tmp.Axes.Add(new DateTimeAxis
				{
					Position = AxisPosition.Bottom,
					//Minimum = DateTimeAxis.ToDouble(dt),
					//Maximum = DateTimeAxis.ToDouble(dt.AddHours(1)),
					IntervalType = DateTimeIntervalType.Minutes,
					MajorGridlineStyle = LineStyle.Solid,
					Angle = 45,
					StringFormat = "HH:mm",
					MajorStep = 1.0 / 24 / 60, // 1/24 = 1 hour, 1/24/2 = 30 minutes
					IsZoomEnabled = true,
					MaximumPadding = 0,
					MinimumPadding = 0,
					TickStyle = TickStyle.None
				});

			var ls = new LineSeries {
				Title = "Line1",
				DataFieldX = "X", 
				DataFieldY = "Y", 
				Color = OxyColors.Blue,
				MarkerType = MarkerType.Circle,
				MarkerSize = 3,
				MarkerStroke = OxyColors.Black,
				MarkerFill = OxyColors.Black,
				MarkerStrokeThickness = 1.5
			};

			tmp.Series.Add(ls);
					
			var clock = new Clock (new DateTimeOffset(1,1,1,0,10,0, TimeSpan.Zero));
			var mock = new LinearMosquito (new TimeSpaned<double>(2, 1.min()));
			clock.Subscribe (mock);

			mock.Receive += (ts) => ls.Points.Add (DateTimeAxis.CreateDataPoint(ts.Timestamp.DateTime, ts.Value));
			mock.Send(Timestamped.Create<double>(20, new DateTimeOffset(1,1,1,0,25,0, TimeSpan.Zero)));

			for (int i = 0; i <= 20; i++) // t = 10 -> t = 30
			{
				clock.Tick (TimeSpan.FromMinutes (1));
				System.Threading.Thread.Sleep (1);
			}

			var plotView = new OxyPlot.GtkSharp.PlotView { Model = tmp };
			plotView.SetSizeRequest(400, 400);
			plotView.Visible = true;

			win.SetSizeRequest(600, 600);
			win.Add(plotView);
			win.Focus = plotView;
			win.Show();
			win.DeleteEvent += (s, a) => {
				Application.Quit ();
				a.RetVal = true;
			};

			win.Show ();
			Application.Run ();
		}
	}
}
