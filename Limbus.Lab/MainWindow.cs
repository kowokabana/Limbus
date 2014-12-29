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
using Limbus.Arduino;

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

		//nodeViewAnalogs.AppendColumn ("Artist", new Gtk.CellRendererText (), "text", 0);
		//nodeViewAnalogs.AppendColumn ("Song Title", new Gtk.CellRendererText (), "text", 1);
		//nodeViewAnalogs.ShowAll ();

		var t0 = DateTimeOffset.UtcNow;
		timePlot = new TimePlot("Plot", 50, t0);
		//HostPID(t0, timePlot);

		var arduino = new Driver();
		var potVal = arduino.AddPin("potVal: ");

		potVal.Receive += (ts) => {
			this.timePlot.AddActual(ts);
			this.timePlot.InvalidatePlot(true);
		};
		
		var timePlotView = new OxyPlot.GtkSharp.PlotView { Model = timePlot };
		timePlotView.SetSizeRequest(1000, 200);
		timePlotView.Visible = true;

		this.SetSizeRequest(1000, 600);
		vbxPlot.Add(timePlotView);

		arduino.Connect("/dev/tty.usbmodem1421", 9600);
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	protected void vScaleSetpoint_Changed(object sender, EventArgs e)
	{
		var setpoint = vscalePoti1.Value.At(clock.Time);//.At(clock.Time);
		controlledMock.Send(setpoint);
		this.timePlot.AddSetpoint(setpoint.Value);//.Value);
	}

	protected void vScalePoti_Changed(object sender, EventArgs e)
	{
		edPoti1.Text = vscalePoti1.Value.ToString();
		edPoti2.Text = vscalePoti2.Value.ToString();
		edPoti3.Text = vscalePoti3.Value.ToString();
		edPoti4.Text = vscalePoti4.Value.ToString();
		edPoti5.Text = vscalePoti5.Value.ToString();
	}

	protected void vScaleSpeed_Changed(object sender, EventArgs e)
	{
		//this.speed = (int)vscaleSpeed.Value;
	}

	protected void vscaleDeadTime_Changed(object sender, EventArgs e)
	{
		//mock.Send(Timestamped.Create(vscaleSetpoint.Value, clock.Time.Add(vscaleDeadTime.Value.min())));
	}

	private void HostPID(DateTimeOffset t0, TimePlot timeplot)
	{
		var linearMock = new LinearMosquito(10.0.In(1.min()), DateTimeOffset.UtcNow);
		controlledMock = linearMock.ControlledBy(new PIDAlgorithm(0.5, 0.5, 0, 0));//WithDelay(5.min());

		clock = new Clock(t0);
		clock.Subscribe(linearMock);
		//clock.Subscribe(delayedMock);

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
	}

	protected void edPoti_KeyReleased (object o, KeyReleaseEventArgs args)
	{
		if (args.Event.Key != Gdk.Key.Return) return;
		if (args.Event.Key == Gdk.Key.Return) {
			this.btnPotiOnOff_Toggled(o, args);
		}
	}

	protected void btnPotiOnOff_Toggled (object sender, EventArgs e)
	{
		var widget = sender as Gtk.Widget;
		if (widget == null) return;

		switch (widget.Name) {
		case "btnPotiOnOff1":
			SetState(edPoti1, btnPotiOnOff1, vscalePoti1);
			break;
		case "edPoti1":
			SetState(edPoti1, btnPotiOnOff1, vscalePoti1, true);
			break;
		case "btnPotiOnOff2":
			SetState(edPoti2, btnPotiOnOff2, vscalePoti2);
			break;
		case "edPoti2":
			SetState(edPoti2, btnPotiOnOff2, vscalePoti2, true);
			break;
		case "btnPotiOnOff3":
			SetState(edPoti3, btnPotiOnOff3, vscalePoti3);
			break;
		case "edPoti3":
			SetState(edPoti3, btnPotiOnOff3, vscalePoti3, true);
			break;
		case "btnPotiOnOff4":		
			SetState(edPoti4, btnPotiOnOff4, vscalePoti4);
			break;
		case "edPoti4":
			SetState(edPoti4, btnPotiOnOff4, vscalePoti4, true);
			break;
		case "btnPotiOnOff5":
			SetState(edPoti5, btnPotiOnOff5, vscalePoti5);
			break;
		case "edPoti5":
			SetState(edPoti5, btnPotiOnOff5, vscalePoti5, true);
			break;
		}
	}

	private static void SetState(Entry ed, ToggleButton btn, VScale poti, bool setActive = false)
	{
		if(setActive) btn.Active = true;
		ed.Sensitive = !btn.Active;
		poti.Value = ParsedOrDefault(ed.Text);
	}

	private static double ParsedOrDefault(string text)
	{
		double parsed;
		if (!double.TryParse(text, out parsed)) return default(double);
		return parsed;
	}
}
