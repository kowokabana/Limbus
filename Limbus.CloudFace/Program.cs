using System;
using AppHarbor;
using System.Security.Authentication;
using System.IO;
using System.Reflection;

namespace Limbus.CloudFace
{
	public class Program
	{
		public static void Main(string[] arguments)
		{
			ConsoleMirror.Initialize();

			try
			{
				Console.WriteLine("AppHarbor background workers rock!");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}

			var output = ConsoleMirror.Captured;
			// do something useful with the output
		}
	}
}
