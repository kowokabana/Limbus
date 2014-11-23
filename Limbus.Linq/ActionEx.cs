using System;

namespace Limbus.Linq
{
	public class Disposable : IDisposable
	{
		private Action action;

		public Disposable(Action action)
		{
			this.action = action;
		}

		public void Dispose()
		{
			action();
		}
	}

	public static class ActionEx
	{
		public static IDisposable AsDisposable(this Action src)
		{
			return new Disposable(src);
		}
	}
}

