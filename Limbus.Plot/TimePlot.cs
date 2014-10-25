using System;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Limbus.Plotter
{
	public class TimePlot : PlotModel
	{
		public LineSeries Line { get; set; }

		public TimePlot (string title, DateTimeOffset tMin, DateTimeOffset tMax, double vMin, double vMax)
			: base (title)
		{
			this.Axes.Add(new LinearAxis 
				{ 
					Position = AxisPosition.Left,
					Minimum = -1,
					Maximum = 30,
					MajorGridlineStyle = LineStyle.Solid,
					MinorGridlineStyle = LineStyle.Dot, 
					TickStyle = TickStyle.Outside 
				});

			this.Axes.Add(new DateTimeAxis
				{
					Position = AxisPosition.Bottom,
					Minimum = DateTimeAxis.ToDouble(tMin.UtcDateTime),
					Maximum = DateTimeAxis.ToDouble(tMax.UtcDateTime.AddMinutes(2)),
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
		}
	}
}

