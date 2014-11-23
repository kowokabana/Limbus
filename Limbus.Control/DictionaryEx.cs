using System;
using System.Collections.Generic;

namespace Limbus.Control
{
	public static class DictionaryEx
	{
		public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> src, TKey key, TValue val)
	  {
			TValue old;
			if (!src.TryGetValue(key, out old)) {
				src.Add(key, val);
				return;
			}
			src [key] = val;
		}
	}
}

