using System.Collections.Generic;
using BubbleSystem;
using DG.Tweening;
using Extensions.Vectors;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GridSystem
{
	public class GridManager : Singleton<GridManager>
	{
		#region References

		[BoxGroup("References"), SerializeField]
		private GridCreateHandler gridCreateHandler;

		[BoxGroup("References")] public List<CellController> cellControllerList;
		public CellController[,] CellController { get; set; } //To reach with coords

		#endregion

		#region Variables

		public Vector2Int GridLength => new(CellController.GetLength(0), CellController.GetLength(1));

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

		[Button]
		public void CheckGridAndProcess()
		{
			//Check empty row count
			var emptyRowCount = 0;
			for (int y = 0; y < GridLength.y; y++)
			{
				var isRowEmpty = true;
				for (int x = 0; x < GridLength.x; x++)
				{
					var cellController = GetCell(x, y);
					if (ReferenceEquals(cellController.bubbleController, null))
						continue;

					isRowEmpty = false;
					break;
				}

				if (isRowEmpty)
					emptyRowCount++;
				else
					break;
			}

			if (emptyRowCount > 1)
			{
				for (int i = 1; i < emptyRowCount; i++)
				{
					gridCreateHandler.DeleteRowFromBottom();
				}
			}
			else if (emptyRowCount == 0)
			{
				//Check bottom row is have bubble or not
				for (int x = 0; x < GridLength.x; x++)
				{
					var cellController = GetCell(x, 0);
					if (ReferenceEquals(cellController.bubbleController, null))
						continue;

					gridCreateHandler.AddExtraRowToBottom();
					break;
				}
			}

			//Check minimum row count
			if (GridLength.y < gridCreateHandler.settings.rowColumnSize.y)
			{
				for (int i = 0; i < gridCreateHandler.settings.rowColumnSize.y - GridLength.y; i++)
				{
					gridCreateHandler.AddExtraRowToTop();
					BubbleManager.Instance.CreateBubbleRow(GridLength.y - 1);
				}
			}
		}

		[Button]
		public void MoveGridDown()
		{
			for (int y = GridLength.y - 1; y >= 0; y--)
			{
				for (int x = 0; x < GridLength.x; x++)
				{
					var cellController = GetCell(x, y);

					var distance = gridCreateHandler.settings.distance;
					var targetCellYPos = cellController.transform.localPosition.y;
					targetCellYPos = y == 0
						? cellController.transform.localPosition.y - distance.y
						: GetCell(x, y - 1).transform.localPosition.y;

					cellController.transform.DOLocalMove(cellController.transform.localPosition.With(y: targetCellYPos),
						0.5f);
				}
			}
		}
	}
}