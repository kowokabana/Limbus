using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Gtk;
using Limbus.Clockwork;
using Limbus.Mosquito;
using Limbus.Plot;
using OxyPlot;
using OxyPlot.Axes;

public partial class MainWindow: Gtk.Window
{
	private LinearMosquito mock;
	private Clock clock;

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		mock = new LinearMosquito (TimeSpaned.Create(2.0, 1.min()));
		var tStart = DateTimeOffset.UtcNow;

		clock = new Clock (tStart);
		clock.Subscribe (mock);

		var timePlot = new TimePlot ("Mosquito Population", tStart, DateTimeOffset.UtcNow, 0, 30);

		mock.Receive += (ts) => {
			timePlot.Line.Points.Add (DateTimeAxis.CreateDataPoint (ts.Timestamp.DateTime, ts.Value));
			timePlot.InvalidatePlot (true);
		};

		Task.Run (() => {
			while(true)
			{
				clock.Tick (1.min());
				Thread.Sleep (1000);
			}
		});

		var plotView = new OxyPlot.GtkSharp.PlotView { Model = timePlot };

		plotView.SetSizeRequest(1000, 600);
		plotView.Visible = true;

		this.SetSizeRequest(1000, 600);

		hbxMain.Add (plotView);
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void btnSendClicked (object sender, EventArgs e)
	{
		var setpoint = double.Parse (edSetpoint.Text);
		mock.Send(Timestamped.Create(setpoint, DateTimeOffset.MinValue));
	}
}
