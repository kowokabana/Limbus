using System;
using System.Linq;

namespace Limbus.Log
{
	public enum Level
	{
		Trace = 0,
		Debug = 1,
		Info = 2,
		Warning = 3,
		Error = 4,
		Fatal = 5
	}

	public static class Logger
	{
		public static void Log(string text)
		{
			Log("offTopic", text, Level.Info);
		}

		public static void Log(string text, Level level)
		{
			Log("offTopic", text, level);
		}

		public static void Log(string topic, string text, Level level)
		{
			Console.WriteLine(string.Format("{0}: {1}", topic, text));
		}
	}
}

