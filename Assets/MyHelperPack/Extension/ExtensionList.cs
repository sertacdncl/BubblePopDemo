using System.Collections.Generic;
using UnityEngine;

namespace Extensions.List
{
	public static class ExtensionList
	{
		public static List<T> Shuffle<T>(this List<T> list)
		{
			var n = list.Count;
			while (n > 1)
			{
				n--;
				var k = Random.Range(0, n);
				(list[k], list[n]) = (list[n], list[k]);
			}

			return list;
		}

		public static T GetRandom<T>(this List<T> origin)
		{
			return origin[Random.Range(0, origin.Count)];
		}

		public static T Pop<T>(this List<T> origin)
		{
			var index = Random.Range(0, origin.Count);
			var current = origin[index];
			origin.RemoveAt(index);
			return current;
		}

		public static bool IsEmpty<T>(this List<T> origin)
		{
			return origin == null || origin.Count == 0;
		}
	}
}