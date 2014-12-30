using System;
using ServiceStack;
using System.IO;

namespace Limbus.Serialization
{
	public static class JSON
	{
		public const string SETTINGSDIR = "settings";

		public static void Save(this object settings)
		{
			var filename = settings.GetType().FullName;
			var json = settings.ToJson();
			var path = System.IO.Path.Combine(SETTINGSDIR, filename);
			if (!Directory.Exists(SETTINGSDIR)) Directory.CreateDirectory(SETTINGSDIR);
			File.WriteAllText(path, json);
		}

		public static T Load<T>() where T : new()
		{
			var filename = typeof(T).FullName;
			var settings = new T();
			var path = System.IO.Path.Combine(SETTINGSDIR, filename);
			if (!File.Exists(path)) settings.Save();
			var json = File.ReadAllText(path);
			settings = json.FromJson<T>();
			return settings;
		}
	}
}

