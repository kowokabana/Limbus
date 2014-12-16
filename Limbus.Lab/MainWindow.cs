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
using Limbus.Control;
using Limbus.API;

public partial class MainWindow: Gtk.Window
{
	private Delayer<double> delayedMock;
	private IControllable<double> controlledMock;
	private Clock clock;
	private int speed = 1000;
	private TimePlot timePlot;

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build();

		var linearMock = new LinearMosquito(10.0.In(1.min()), DateTimeOffset.UtcNow);
		controlledMock = linearMock.ControlledBy(new PIDAlgorithm(0.5, 1, 0.5));//WithDelay(5.min());
		var tStart = DateTimeOffset.UtcNow;

		clock = new Clock(tStart);
		clock.Subscribe(linearMock);
		//clock.Subscribe(delayedMock);

		timePlot = new TimePlot ("Mosquito Plot", 50, tStart);

		// this stuff runs in another task
		linearMock.Receive += (ts) => {
			timePlot.AddActual(ts);
			timePlot.InvalidatePlot(true);
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

		vbxPlot.Add(plotView);
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	protected void vScaleSetpoint_Changed(object sender, EventArgs e)
	{
		var setpoint = vscaleSetpoint.Value.At(clock.Time);//.At(clock.Time);
		controlledMock.Send(setpoint);
		this.timePlot.AddSetpoint(setpoint.Value);//.Value);
	}

	protected void vScaleSpeed_Changed(object sender, EventArgs e)
	{
		this.speed = (int)vscaleSpeed.Value;
	}

	protected void vscaleDeadTime_Changed(object sender, EventArgs e)
	{
		//mock.Send(Timestamped.Create(vscaleSetpoint.Value, clock.Time.Add(vscaleDeadTime.Value.min())));
	}
}
