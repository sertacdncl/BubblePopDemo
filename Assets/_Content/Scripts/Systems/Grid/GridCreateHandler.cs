using System;
using System.Collections.Generic;
using BubbleSystem;
using Pooling;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GridSystem
{
	public class GridCreateHandler : MonoBehaviour
	{
		#region References

		[BoxGroup("References")] public GridSettings settings;

		[BoxGroup("References"), SerializeField]
		private Transform gridHolder;

		private GridManager _gridManager;

		#endregion

		#region Variables

		#endregion

		#region Properties

		#endregion

		private void Start()
		{
			_gridManager = GridManager.Instance;
			CreateGrid();
			BubbleManager.Instance.PrepareStart();
		}

		[Button]
		public void CreateGrid()
		{
			_gridManager.CellController = new CellController[(int)settings.rowColumnSize.x, (int)settings.rowColumnSize.y];
			_gridManager.cellControllerList = new List<CellController>();

			for (int y = 0; y < settings.rowColumnSize.y; y++)
			{
				for (int x = 0; x < settings.rowColumnSize.x; x++)
				{
					var coordinate = new Vector2Int(x, y);

					//This is for shift the rows
					//WARNING: This must be like this. If you change this, you must change the neighbour system
					var separation = y % 2 == 0 ? 0.5f : 0f;

					Vector3 cellPos = new Vector3(x * settings.distance.x + separation, y * settings.distance.y, 0);

					GameObject cell = PoolingManager.Instance.GetObjectFromPool("GridCell");
					cell.transform.position = cellPos;
					cell.transform.rotation = Quaternion.identity;
					cell.transform.SetParent(gridHolder, false);
					cell.name = $"Cell [{x},{y}]";

					CellController cellController = cell.GetComponent<CellController>();
					cellController.coordinate = coordinate;
					cellController.Neighbours = new CellNeighbours();

					_gridManager.CellController[x, y] = cellController;
					_gridManager.cellControllerList.Add(cellController);
				}
			}

			SetTileNeighbours();
		}

		[Button]
		public void AddExtraRowToBottom()
		{
			var cloneCellController = (CellController[,])_gridManager.CellController.Clone();
			var rowCount = cloneCellController.GetLength(0);
			var columnCount = cloneCellController.GetLength(1);
			var newCellControllerArray = new CellController[rowCount, columnCount+1];
			
			var newCellControllerList = new List<CellController>();

			var separation = Math.Abs(cloneCellController[0, 0].transform.localPosition.x - 0.5f) < 0.1f ? 0f : 0.5f;
			
			for (int x = 0; x < rowCount; x++)
			{
				var bottomCellPos = cloneCellController[x, 0].transform.localPosition;
				
				var coordinate = new Vector2Int(x, 0);
				Vector3 cellPos = new Vector3(x * settings.distance.x + separation, bottomCellPos.y - settings.distance.y, 0);
				GameObject cell = PoolingManager.Instance.GetObjectFromPool("GridCell");
				cell.transform.position = cellPos;
				cell.transform.rotation = Quaternion.identity;
				cell.transform.SetParent(gridHolder, false);
				cell.transform.SetSiblingIndex(x);
				cell.name = $"Cell [{x},{0}]";
				
				CellController cellController = cell.GetComponent<CellController>();
				cellController.coordinate = coordinate;
				cellController.Neighbours = new CellNeighbours();
				
				newCellControllerArray[x, 0] = cellController;
				newCellControllerList.Add(cellController);
			}

			foreach (var cellController in _gridManager.cellControllerList)
			{
				var x = cellController.coordinate.x;
				var y = cellController.coordinate.y;
				cellController.coordinate = new Vector2Int(x, y+1);
				cellController.name = $"Cell [{x},{y+1}]";
				newCellControllerArray[x, y+1] = cellController;
				newCellControllerList.Add(cellController);
			}
			
			_gridManager.CellController = newCellControllerArray;
			_gridManager.cellControllerList = newCellControllerList;

			SetTileNeighbours();
		}
		
		[Button]
		public void AddExtraRowToTop()
		{
			var cloneCellController = (CellController[,])_gridManager.CellController.Clone();
			var rowCount = cloneCellController.GetLength(0);
			var columnCount = cloneCellController.GetLength(1);
			
			var newCellControllerArray = new CellController[rowCount, columnCount+1];

			for (int y = 0; y < columnCount; y++)
			{
				for (int x = 0; x < rowCount; x++)
				{
					newCellControllerArray[x,y] = cloneCellController[x,y];
				}
			}

			var separation = Math.Abs(cloneCellController[0, columnCount-1].transform.localPosition.x - 0.5f) < 0.1f ? 0f : 0.5f;
			
			for (int x = 0; x < rowCount; x++)
			{
				var topCellPos = cloneCellController[x, columnCount-1].transform.localPosition;
				
				var coordinate = new Vector2Int(x, columnCount);
				Vector3 cellPos = new Vector3(x * settings.distance.x + separation, topCellPos.y + settings.distance.y, 0);
				GameObject cell = PoolingManager.Instance.GetObjectFromPool("GridCell");
				cell.transform.position = cellPos;
				cell.transform.rotation = Quaternion.identity;
				cell.transform.SetParent(gridHolder, false);
				cell.transform.SetSiblingIndex(_gridManager.cellControllerList.Count + x);
				cell.name = $"Cell [{x},{columnCount}]";
				
				CellController cellController = cell.GetComponent<CellController>();
				cellController.coordinate = coordinate;
				cellController.Neighbours = new CellNeighbours();
				
				newCellControllerArray[x, columnCount] = cellController;
				_gridManager.cellControllerList.Add(cellController);
			}
			_gridManager.CellController = newCellControllerArray;
		}

		[Button]
		public void DeleteRowFromBottom()
		{
			var cloneCellController = (CellController[,])_gridManager.CellController.Clone();
			var rowCount = cloneCellController.GetLength(0);
			var columnCount = cloneCellController.GetLength(1);
			var newCellControllerArray = new CellController[rowCount, columnCount-1];
			var newCellControllerList = new List<CellController>();

			for (int x = 0; x < rowCount; x++)
			{
				_gridManager.CellController[x,0].transform.SetParent(PoolingManager.Instance.GetPoolHolder("GridCell"));
				PoolingManager.Instance.ReturnObjectToPool(_gridManager.CellController[x,0].gameObject, "GridCell");
			}
			
			for (int y = 1; y < columnCount; y++)
			{
				for (int x = 0; x < rowCount; x++)
				{
					var cellController = cloneCellController[x,y];
					cellController.coordinate = new Vector2Int(x, y-1);
					cellController.name = $"Cell [{x},{y-1}]";
					newCellControllerArray[x, y-1] = cellController;
					newCellControllerList.Add(cellController);
				}
			}
			
			_gridManager.CellController = newCellControllerArray;
			_gridManager.cellControllerList = newCellControllerList;
			
			SetTileNeighbours();
		}

		public void SetTileNeighbours()
		{
			for (int x = 0; x < _gridManager.GridLength.x; x++)
			{
				for (int y = 0; y < _gridManager.GridLength.y; y++)
				{
					var cell = _gridManager.CellController;
					CellController baseCell = _gridManager.CellController[x, y];
					Vector2Int myCoord = baseCell.coordinate;

					baseCell.Neighbours.Left = _gridManager.GetCell(myCoord.x - 1, myCoord.y);
					baseCell.Neighbours.Right = _gridManager.GetCell(myCoord.x + 1, myCoord.y);

					//2X
					if (y % 2 == 0)
					{
						baseCell.Neighbours.UpRight = _gridManager.GetCell(myCoord.x + 1, myCoord.y + 1);
						baseCell.Neighbours.UpLeft = _gridManager.GetCell(myCoord.x, myCoord.y + 1);
						baseCell.Neighbours.DownRight = _gridManager.GetCell(myCoord.x + 1, myCoord.y - 1);
						baseCell.Neighbours.DownLeft = _gridManager.GetCell(myCoord.x, myCoord.y - 1);
					}
					else
					{
						baseCell.Neighbours.UpRight = _gridManager.GetCell(myCoord.x, myCoord.y + 1);
						baseCell.Neighbours.UpLeft = _gridManager.GetCell(myCoord.x - 1, myCoord.y + 1);
						baseCell.Neighbours.DownRight = _gridManager.GetCell(myCoord.x, myCoord.y - 1);
						baseCell.Neighbours.DownLeft = _gridManager.GetCell(myCoord.x - 1, myCoord.y - 1);
					}
				}
			}
		}
	}
}