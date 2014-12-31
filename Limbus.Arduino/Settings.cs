using System;
using System.IO;

namespace Limbus.Arduino
{
	public class Settings
	{
		public string SerialPort { get; set; }
		public int BaudRate { get; set; }

		public string AnalogIn0 { get; set; }
		public string AnalogIn1 { get; set; }
		public string AnalogIn2 { get; set; }
		public string AnalogIn3 { get; set; }
		public string AnalogIn4 { get; set; }
		public string AnalogIn5 { get; set; }

		public string AnalogOut3 { get; set; }
		public string AnalogOut5 { get; set; }
		public string AnalogOut6 { get; set; }
		public string AnalogOut9 { get; set; }
		public string AnalogOut10 { get; set; }
		public string AnalogOut11 { get; set; }

		public string DigitalIn8 { get; set; }
		public string DigitalIn12 { get; set; }
		public string DigitalIn13 { get; set; }

		public string DigitalOut2 { get; set; }
		public string DigitalOut4 { get; set; }
		public string DigitalOut7 { get; set; }

		public Settings()
		{
			SerialPort = "/dev/tty.usbmodem1421";
			BaudRate = 9600;

			AnalogIn0 = "A0:";
			AnalogIn1 = "A1:";
			AnalogIn2 = "A2:";
			AnalogIn3 = "A3:";
			AnalogIn4 = "A4:";
			AnalogIn5 = "A5:";

			AnalogOut3 = "3";
			AnalogOut5 = "5";
			AnalogOut6 = "6";
			AnalogOut9 = "9";
			AnalogOut10 = "10";
			AnalogOut11 = "11";

			DigitalIn8 = "8";
			DigitalIn12 = "12";
			DigitalIn13 = "13";

			DigitalOut2 = "2";
			DigitalOut4 = "4";
			DigitalOut7 = "7";
		}
	}
}

