using System;
using System.Threading;
using System.Threading.Tasks;
using Gtk;
using Limbus.Clockwork;
using Limbus.Mosquito;
using Limbus.Plot;
using OxyPlot;
using OxyPlot.Axes;
using System.Linq;
using MoreLinq;

public partial class MainWindow: Gtk.Window
{
	private LinearMosquito mock;
	private Clock clock;
	private int speed = 1000;

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		mock = new LinearMosquito(TimeSpaned.Create(2.0, 1.min()));
		var tStart = DateTimeOffset.UtcNow;

		clock = new Clock(tStart);
		clock.Subscribe(mock);

		var timePlot = new TimePlot ("Mosquito Plot", 50, tStart);

		// this stuff runs in another task
		mock.Receive += (ts) => {
			timePlot.AddPoint(ts);
			timePlot.InvalidatePlot (true);
		};

		Task.Run (() => {
			while(true)
			{
				clock.Tick (1.min());
				Thread.Sleep (speed);
			}
		});

		var plotView = new OxyPlot.GtkSharp.PlotView { Model = timePlot };

		plotView.SetSizeRequest(1000, 200);
		plotView.Visible = true;

		this.SetSizeRequest(1000, 600);

		vbxPlot.Add (plotView);
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void vScaleSetpoint_Changed (object sender, EventArgs e)
	{
		mock.Send(Timestamped.Create(vscaleSetpoint.Value, clock.Time.Add(vscaleDeadTime.Value.min())));
	}

	protected void vScaleSpeed_Changed (object sender, EventArgs e)
	{
		this.speed = (int)vscaleSpeed.Value;
	}

	protected void vscaleDeadTime_Changed (object sender, EventArgs e)
	{
		mock.Send(Timestamped.Create(vscaleSetpoint.Value, clock.Time.Add(vscaleDeadTime.Value.min())));
	}
}
