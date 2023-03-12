using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace GridSystem
{
	public class GridCreateHandler : MonoBehaviour
	{
		#region References

		[Header("References"), SerializeField] private GridSettings settings;
		[SerializeField] private Transform gridHolder;
		private GridManager _gridManager;

		#endregion

		#region Variables

		#endregion

		#region Properties

		#endregion

		private void Start()
		{
			_gridManager = GridManager.Instance;
		}

		[Button]
		public void CreateGrid()
		{
			_gridManager.CellController = new CellController[(int)settings.rowColumnSize.x, (int)settings.rowColumnSize.y];
			_gridManager.cellControllerList = new List<CellController>();

			for (int x = 0; x < settings.rowColumnSize.x; x++)
			{
				for (int y = 0; y < settings.rowColumnSize.y; y++)
				{
					var coordinate = new Vector2Int(x, y);

					//This is for shift the rows
					//WARNING: This must be like this. If you change this, you must change the neighbour system
					var separation = y % 2 == 0 ? 0.5f : 0f;

					Vector3 cellPos = new Vector3(x * settings.distance.x + separation, y * settings.distance.y, 0);

					GameObject cell = Instantiate(settings.cellPrefab, cellPos, Quaternion.identity);
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

		public void SetTileNeighbours()
		{
			for (int x = 0; x < settings.rowColumnSize.x; x++)
			{
				for (int y = 0; y < settings.rowColumnSize.y; y++)
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