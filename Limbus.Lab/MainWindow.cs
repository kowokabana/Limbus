using System;
using Gtk;
using OxyPlot.GtkSharp;
using OxyPlot;

public partial class MainWindow: Gtk.Window
{
	public MainWindow (PlotModel plotModel) : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		var plotView = new OxyPlot.GtkSharp.PlotView { Model = plotModel };

		plotView.SetSizeRequest(1000, 600);
		plotView.Visible = true;

		this.SetSizeRequest(1000, 600);

		hbxMain.Add (plotView);

		//this.Add(plotView);
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void btnSendClicked (object sender, EventArgs e)
	{
		var setpoint = double.Parse (edSetpoint.Text);
	}
}
