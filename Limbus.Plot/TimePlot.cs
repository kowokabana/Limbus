using System;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using Limbus.Clockwork;
using System.Collections.Generic;

namespace Limbus.Plot
{
	public class TimePlot : PlotModel
	{
		public List<LineSeries> Lines { get; private set; }
		public DateTimeAxis TimeAxis { get; private set; }

		private int width = 50;
		private double setpoint = 0;

		public void AddActual(Timestamped<double> point)
		{
			this.Lines[0].Points.Add(DateTimeAxis.CreateDataPoint (point.Timestamp.UtcDateTime, point.Value));
			this.Lines[1].Points.Add(DateTimeAxis.CreateDataPoint (point.Timestamp.UtcDateTime, setpoint));
			if (this.Lines[0].Points.Count > width) this.Lines.ForEach(l => l.Points.RemoveAt(0));
		}

		public void AddSetpoint(double setpoint)
		{
			this.setpoint = setpoint;
		}

		public TimePlot (string title, int width, DateTimeOffset start)
			: base (title)
		{
			if (width < 0) throw new ArgumentException ("width");
			this.Lines = new List<LineSeries>();

			this.width = width;

			this.Axes.Add(new LinearAxis
				{ 
					Position = AxisPosition.Left,
					MajorGridlineStyle = LineStyle.Solid,
					MinorGridlineStyle = LineStyle.Dot,
					TickStyle = TickStyle.Outside,
					Maximum = 500, // limits the max of the y axis
					Minimum = 0,
					AbsoluteMaximum = 500, // limits the max of the y axis
					AbsoluteMinimum = 0
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

			var actuals = new LineSeries {
				Title = "Actuals",
				DataFieldX = "X",
				DataFieldY = "Y", 
				Color = OxyColors.Blue,
				MarkerType = MarkerType.Circle,
				MarkerSize = 1,
				MarkerStroke = OxyColors.Black,
				MarkerFill = OxyColors.Black,
				MarkerStrokeThickness = 1.5
			};

			var setpoints = new StairStepSeries {
				Title = "Setpoints",
				DataFieldX = "X",
				DataFieldY = "Y", 
				Color = OxyColors.Red,
				MarkerType = MarkerType.None,
				MarkerSize = 2,
				MarkerStroke = OxyColors.Black,
				MarkerFill = OxyColors.Black,
				MarkerStrokeThickness = 1.5
			};

			this.Lines.Add(actuals);
			this.Lines.Add(setpoints);
			this.Series.Add(actuals);
			this.Series.Add(setpoints);

			for (int i = 0; i <= width; i++) {
				AddActual(0.0.At(start.AddMinutes(-width).AddMinutes(i)));
			}
		}
	}
}

