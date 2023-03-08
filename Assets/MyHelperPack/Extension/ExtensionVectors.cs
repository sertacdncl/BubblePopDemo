using UnityEngine;

namespace Extensions.Vectors
{
	public static class ExtensionVectors
	{
		public static Vector3 With(this Vector3 origin, float? x = null, float? y = null, float? z = null)
		{
			return new Vector3(x ?? origin.x, y ?? origin.y, z ?? origin.z);
		}

		public static Vector2 With(this Vector2 origin, float? x = null, float? y = null)
		{
			return new Vector3(x ?? origin.x, y ?? origin.y);
		}

		public static Vector3 WithAddX(this Vector3 origin, float x)
		{
			return new Vector3(origin.x + x, origin.y, origin.z);
		}

		public static Vector3 WithAddY(this Vector3 origin, float y)
		{
			return new Vector3(origin.x, origin.y + y, origin.z);
		}

		public static Vector3 WithAddZ(this Vector3 origin, float z)
		{
			return new Vector3(origin.x, origin.y, origin.z + z);
		}

		public static Vector2 WithAddX(this Vector2 origin, float x)
		{
			return new Vector2(origin.x + x, origin.y);
		}

		public static Vector2 WithAddY(this Vector2 origin, float y)
		{
			return new Vector2(origin.x, origin.y + y);
		}
	}
}