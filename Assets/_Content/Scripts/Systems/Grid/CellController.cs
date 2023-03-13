using System.Collections.Generic;
using BubbleSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GridSystem
{
	public class CellController : MonoBehaviour
	{
		#region References

		[SerializeField] private SpriteRenderer spriteRenderer;
		public CellNeighbours Neighbours = new();
		public BubbleController bubbleController;

		#endregion

		#region Variables

		[Unity.Collections.ReadOnly] public Vector2Int coordinate;

		#endregion

		#region Properties

		#endregion


		private bool IsNeighbour(CellController cell)
		{
			foreach (var cellController in cell.Neighbours.GetAllNeighbour())
			{
				if (cellController == this)
					return true;
			}

			return false;
		}

		[Button]
		public void ShowNeighbours()
		{
			foreach (var cellController in Neighbours.GetAllNeighbour())
			{
				cellController.spriteRenderer.color =
					cellController.spriteRenderer.color == Color.white ? Color.red : Color.white;
			}
		}
	}
}