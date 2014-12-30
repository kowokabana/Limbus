using System;
using System.IO;

namespace Limbus.Arduino
{
	public class Settings
	{
		public string SerialPort { get; set; }
		public int BaudRate { get; set; }
		public string AnalogIn1 { get; set; }
		public string AnalogIn2 { get; set; }

		public Settings()
		{
			SerialPort = "/dev/tty.usbmodem1421";
			BaudRate = 9600;
			AnalogIn1 = "potVal: ";
			AnalogIn2 = "engineVal: ";
		}
	}
}

