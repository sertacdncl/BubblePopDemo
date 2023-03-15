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
		
		public static Direction GetOppositeDirection(Direction direction)
		{
			return direction switch
			{
				Direction.Left => Direction.Right,
				Direction.Right => Direction.Left,
				Direction.UpRight => Direction.DownLeft,
				Direction.UpLeft => Direction.DownRight,
				Direction.DownRight => Direction.UpLeft,
				Direction.DownLeft => Direction.UpRight,
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
			};
		}
	}
}