using System;
using System.IO;
using ServiceStack;

namespace Limbus.Lab
{
	public class Settings
	{
		public string Poti1 { get; set; }
		public string Poti2 { get; set; }
		public string Poti3 { get; set; }
		public string Poti4 { get; set; }
		public string Poti5 { get; set; }

		public int PlotHeight { get; set; }
		public int PlotWidth { get; set; }

		public int PotiMin { get; set; }
		public int PotiMax { get; set; }

		public Settings()
		{
			Poti1 = "Poti1";
			Poti2 = "Poti2";
			Poti3 = "Poti3";
			Poti4 = "Poti4";
			Poti5 = "Poti5";

			PlotHeight = 1000;
			PlotWidth = 200;

			PotiMin = 0;
			PotiMax = 255;
		}
	}
}

