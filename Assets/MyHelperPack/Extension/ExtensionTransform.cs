using System.Collections.Generic;

namespace Extensions.Transform
{
	public static class ExtensionTransform
	{
		public static List<UnityEngine.Transform> GetChildren(this UnityEngine.Transform parent)
		{
			var list = new List<UnityEngine.Transform>();
			for (var i = 0; i < parent.childCount; i++)
			{
				list.Add(parent.GetChild(i));
			}

			return list;
		}
	}
}