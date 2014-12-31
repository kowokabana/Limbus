using System;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using Limbus.Clockwork;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Limbus.Plot
{
	public class TimePlot : PlotModel
	{
		public List<LineSeries> ActualLines { get; private set; }
		public List<StairStepSeries> SetpointLines { get; private set; }

		private Dictionary<string, double> setpoints = new Dictionary<string, double>();

		public DateTimeAxis TimeAxis { get; private set; }
		private int width = 50;

		private void Slide()		{
			var lines = ActualLines.Union(SetpointLines);
			if (lines.Any(l => l.Points.Count > width)) {
				lines.ForEach(l => l.Points.RemoveAt(0));
			}
		}

		public void AddActual(string title, Timestamped<double> actual)
		{
			var line = ActualLines.First(l => l.Title == title);
			line.Points.Add(DateTimeAxis.CreateDataPoint(actual.Timestamp.UtcDateTime, actual.Value));
		}

		public void AddSetpoint(string title, Timestamped<double> setpoint)
		{
			setpoints[title] = setpoint.Value;
			var line = SetpointLines.First(l => l.Title == title);
			line.Points.Add(DateTimeAxis.CreateDataPoint(setpoint.Timestamp.UtcDateTime, setpoint.Value));
		}

		public void AddActualLine(string title)
		{
			var actuals = new LineSeries {
				Title = title,
				DataFieldX = "X",
				DataFieldY = "Y", 
				Color = OxyColors.Blue,
				MarkerType = MarkerType.Circle,
				MarkerSize = 1,
				MarkerStroke = OxyColors.Black,
				MarkerFill = OxyColors.Black,
				MarkerStrokeThickness = 1.5
			};

			this.ActualLines.Add(actuals);
			this.Series.Add(actuals);
		}

		public void AddSetpointLine(string title)
		{
			var setpoints = new StairStepSeries {
				Title = title,
				DataFieldX = "X",
				DataFieldY = "Y", 
				Color = OxyColors.Red,
				MarkerType = MarkerType.None,
				MarkerSize = 2,
				MarkerStroke = OxyColors.Black,
				MarkerFill = OxyColors.Black,
				MarkerStrokeThickness = 1.5
			};

			this.setpoints.Add(title, 0);
			this.SetpointLines.Add(setpoints);
			this.Series.Add(setpoints);
		}

		public TimePlot(string title, int width, int height, DateTimeOffset start)
			: base (title)
		{
			if (width < 0) throw new ArgumentException ("width");
			this.width = width;

			this.ActualLines = new List<LineSeries>();
			this.SetpointLines = new List<StairStepSeries>();

			this.Axes.Add(new LinearAxis
				{ 
					Position = AxisPosition.Left,
					MajorGridlineStyle = LineStyle.Solid,
					MinorGridlineStyle = LineStyle.Dot,
					TickStyle = TickStyle.Outside,
					Maximum = height, // limits the max of the y axis
					Minimum = 0,
					AbsoluteMaximum = height, // limits the max of the y axis
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

			AddActualLine("DummyLine");

			for (int i = 0; i <= width; i++) {
				ActualLines.ForEach(l => AddActual(l.Title, (0.0.At(start.AddMinutes(-width).AddMinutes(i)))));
			}
		}
	}
}

