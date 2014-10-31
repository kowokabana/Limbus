using System;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Reactive;

namespace Limbus.Plot
{
	public class TimePlot : PlotModel
	{
		public LineSeries Line { get; private set; }
		public DateTimeAxis TimeAxis { get; private set; }

		private int width = 50;

		public void AddPoint(Timestamped<double> point)
		{
			this.Line.Points.Add (DateTimeAxis.CreateDataPoint (point.Timestamp.UtcDateTime, point.Value));
			if(this.Line.Points.Count > width) this.Line.Points.RemoveAt(0);
		}

		public TimePlot (string title, int width, DateTimeOffset start)
			: base (title)
		{
			this.width = width;

			this.Axes.Add(new LinearAxis
				{ 
					Position = AxisPosition.Left,
					MajorGridlineStyle = LineStyle.Solid,
					MinorGridlineStyle = LineStyle.Dot,
					TickStyle = TickStyle.Outside
				});

			this.TimeAxis = new DateTimeAxis {
				Position = AxisPosition.Bottom,
				IntervalType = DateTimeIntervalType.Minutes,
				MajorGridlineStyle = LineStyle.Solid,
				Angle = 45,
				StringFormat = "HH:mm",
				MajorStep = 1.0 / 24 / 60, // 1/24 = 1 hour, 1/24/2 = 30 minutes
				IsZoomEnabled = true,
				MaximumPadding = 0,
				MinimumPadding = 0,
				TickStyle = TickStyle.None,
			};

			this.Axes.Add(TimeAxis);

			var line = new LineSeries {
				Title = "Line1",
				DataFieldX = "X", 
				DataFieldY = "Y", 
				Color = OxyColors.Blue,
				MarkerType = MarkerType.Circle,
				MarkerSize = 2,
				MarkerStroke = OxyColors.Black,
				MarkerFill = OxyColors.Black,
				MarkerStrokeThickness = 1.5
			};

			this.Line = line;
			this.Series.Add(line);

			for (int i = 0; i <= width; i++) {
				AddPoint (Timestamped.Create (0.0, start.AddMinutes(-width).AddMinutes(i)));
			}
		}
	}
}

