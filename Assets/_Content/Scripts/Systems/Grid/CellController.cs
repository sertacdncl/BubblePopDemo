using System.Collections.Generic;
using BubbleSystem;
using DG.Tweening;
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

		public void ShootEffectToNeighbours()
		{
			foreach (var direction in Directions.AllDirections)
			{
				var neighbour = Neighbours.GetNeighbour(direction);
				
				if(ReferenceEquals(neighbour,null))
					continue;
				
				if(ReferenceEquals(neighbour.bubbleController, null))
					continue;
				
				neighbour.ShootEffect(direction);
			}
		}
		private void ShootEffect(Direction direction)
		{
			var targetPosition = bubbleController.transform.localPosition;
			var moveSize = 0.1f;
			switch (direction)
			{
				case Direction.UpRight:
					targetPosition += new Vector3(moveSize, moveSize, 0);
					break;
				case Direction.UpLeft:
					targetPosition += new Vector3(-moveSize, moveSize, 0);
					break;
				case Direction.DownRight:
					targetPosition += new Vector3(moveSize, -moveSize, 0);
					break;
				case Direction.DownLeft:
					targetPosition += new Vector3(-moveSize, -moveSize, 0);
					break;
				case Direction.Left:
					targetPosition += Vector3.left * moveSize;
					break;
				case Direction.Right:
					targetPosition += Vector3.right * moveSize;
					break;
			}

			var pathList = new List<Vector3>()
			{
				targetPosition,
				Vector3.zero
			};
			var sequence = DOTween.Sequence();
			sequence.Append(bubbleController.transform.DOLocalPath(pathList.ToArray(), 0.1f).SetEase(Ease.Linear));
		}
	}
}