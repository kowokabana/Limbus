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
	private Delayer<double> delayedMock;
	private Clock clock;
	private int speed = 1000;

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		var linearMock = new LinearMosquito(2.0.In(1.min()), DateTimeOffset.UtcNow);
		delayedMock = linearMock.WithDelay(5.min());
		var tStart = DateTimeOffset.UtcNow;

		clock = new Clock(tStart);
		clock.Subscribe(linearMock);
		clock.Subscribe(delayedMock);

		var timePlot = new TimePlot ("Mosquito Plot", 50, tStart);

		// this stuff runs in another task
		linearMock.Receive += (ts) => {
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
		var setpoint = vscaleSetpoint.Value.At(clock.Time).At(clock.Time);
		delayedMock.Send(setpoint);
	}

	protected void vScaleSpeed_Changed (object sender, EventArgs e)
	{
		this.speed = (int)vscaleSpeed.Value;
	}

	protected void vscaleDeadTime_Changed (object sender, EventArgs e)
	{
		//mock.Send(Timestamped.Create(vscaleSetpoint.Value, clock.Time.Add(vscaleDeadTime.Value.min())));
	}
}
