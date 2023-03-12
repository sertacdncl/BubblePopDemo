using System;
using System.Collections.Generic;

namespace GridSystem
{
	[Serializable]
	public class CellNeighbours
	{
		public CellController Right;
		public CellController Left;
		public CellController UpRight;
		public CellController UpLeft;
		public CellController DownRight;
		public CellController DownLeft;
		
		public List<CellController> GetAllNeighbour()
		{
			var list = new List<CellController>();
			foreach (var direction in Directions.AllDirections)
			{
				CellController cellController = GetNeighbour(direction);
				if (!ReferenceEquals(cellController, null))
					list.Add(cellController);
			}

			return list;
		}

		public List<Direction> GetAllNeighbourDirection()
		{
			var list = new List<Direction>();
			foreach (var direction in Directions.AllDirections)
			{
				CellController cellController = GetNeighbour(direction);
				if (!ReferenceEquals(cellController, null))
					list.Add(direction);
			}

			return list;
		}
		

		public CellController GetNeighbour(Direction direction)
		{
			return direction switch
			{
				Direction.Left => Left,
				Direction.Right => Right,
				Direction.UpRight =>  UpRight != null ? UpRight : null,
				Direction.UpLeft => UpLeft != null ? UpLeft : null,
				Direction.DownRight => DownRight != null ? DownRight : null,
				Direction.DownLeft => DownLeft != null ? DownLeft : null,
				_ => null
			};
		}
	}
}