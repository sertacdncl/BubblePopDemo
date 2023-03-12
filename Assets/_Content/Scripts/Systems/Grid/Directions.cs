using System;
using UnityEngine;

namespace GridSystem
{
	public static class Directions
	{
		public static Direction[] AllDirections =
		{
			Direction.Left, Direction.Right, Direction.DownRight, Direction.DownLeft,
			Direction.UpLeft, Direction.UpRight
		};

		public static Vector2 GetDirectionValue(Direction direction)
		{
			return direction switch
			{
				Direction.Left => Vector2.left,
				Direction.Right => Vector2.right,
				Direction.UpRight => new Vector2(1, 1),
				Direction.UpLeft => new Vector2(-1, 1),
				Direction.DownRight => new Vector2(1, -1),
				Direction.DownLeft => new Vector2(-1, -1),
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
			};
		}
	}
}