using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
	public class GridManager : Singleton<GridManager>
	{
		#region References

		[Header("References")] public List<CellController> cellControllerList;
		public CellController[,] CellController { get; set; } //To reach with coords

		#endregion

		#region Variables

		#endregion

		#region Properties

		#endregion

		public CellController GetCell(Vector2Int coordinate) =>
			cellControllerList.Find(x => x.coordinate == coordinate);

		public CellController GetCell(int x, int y)
		{
			if (x < 0 || y < 0 || x >= CellController.GetLength(0) || y >= CellController.GetLength(1))
				return null;

			return CellController[x, y];
		}


		//To find direction is cross or not
		public static bool IsCross(Direction direction)
		{
			return direction is Direction.DownLeft or Direction.DownRight or Direction.UpLeft or Direction.UpRight;
		}
	}
}