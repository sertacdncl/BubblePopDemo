using System.Collections.Generic;
using DG.Tweening;
using Extensions;
using GridSystem;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace BubbleSystem
{
	public class BubbleShootPredictionHandler : MonoBehaviour
	{
		#region References

		[BoxGroup("References"), SerializeField]
		private Camera mainCamera;

		[BoxGroup("References"), SerializeField]
		private Transform shooterTransform;

		[BoxGroup("References")] public SpriteRenderer bubbleIndicator;

		[BoxGroup("References"), SerializeField]
		private LineRenderer lineRenderer;

		[BoxGroup("References"), SerializeField]
		private LayerMask raycastLayers;

		[BoxGroup("References"), ReadOnly]
		public CellController lastTargetCell;
		
		[BoxGroup("References"), ReadOnly]
		public Direction lastTargetCellDirection;
		
		[BoxGroup("References"), ReadOnly]
		public List<Vector3> lastRayPath;

		#endregion

		#region Variables

		private Vector2 _lastHitPoint;
		private Vector3 _lastIndicatorPos;

		#endregion

		public void OnMouseButton()
		{
			var rayPath = GetTargetDestination();
			SetVisuals(rayPath);
		}
		
		public void OnMouseButtonUp()
		{
			UpdateLastRayPath();
			SetVisuals(null);
		}

		private void UpdateLastRayPath()
		{
			lastRayPath.Clear();
			var pathListTemp = new List<Vector3>();
			for (int i = 0; i < lineRenderer.positionCount; i++)
			{
				var position = lineRenderer.GetPosition(i);
				pathListTemp.Add(position);
			}

			lastRayPath = pathListTemp;
		}

		private List<Vector3> GetTargetDestination()
		{
			var shooterPos = shooterTransform.position;
			var mousePos = GetMousePosition(shooterPos);

			var ray = new Ray2D(shooterPos, mousePos);
			var rayPath = new List<Vector3>();
			var raycastHit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, raycastLayers);
			lastTargetCell = null; ;
			lastRayPath.Clear();
			if (raycastHit)
			{
#if UNITY_EDITOR
				Debug.DrawLine(shooterTransform.position, raycastHit.centroid, Color.red);
#endif
				//Checking if the raycast hit a bubble
				if (raycastHit.collider.CompareTag("Bubble"))
				{
					// Add shooter position to ray path
					rayPath.Add(shooterPos);
					rayPath = GetTargetRayDestination(raycastHit, rayPath);
				}
				else if (raycastHit.collider.CompareTag("Wall"))
				{
					// Get the rotation needed to deflect the ray off the wall
					var deflectRotation = Quaternion.FromToRotation(-ray.direction, raycastHit.normal);
					// Get the deflected direction
					var deflectDirection = deflectRotation * raycastHit.normal;
					// Create a new ray with the deflected direction
					var deflectRay = new Ray(raycastHit.centroid, deflectDirection);

					var bounceHit =
						Physics2D.RaycastAll(deflectRay.origin, deflectRay.direction, Mathf.Infinity, raycastLayers);
					var bubbleHit = new RaycastHit2D();

					foreach (var hit in bounceHit)
					{
						if (hit.collider.tag == "Bubble")
						{
#if UNITY_EDITOR
							Debug.DrawLine(deflectRay.origin, hit.point, Color.yellow);
#endif
							// Add shooter position and deflected ray origin to ray path
							rayPath.Add(shooterPos);
							rayPath.Add(deflectRay.origin);
							bubbleHit = hit;
							break;
						}
					}

					if (!bubbleHit)
						return new List<Vector3>();

					rayPath = GetTargetRayDestination(bubbleHit, rayPath);
				}
			}

			return rayPath;
		}

		private List<Vector3> GetTargetRayDestination(RaycastHit2D bubbleHit, List<Vector3> rayPath)
		{
			// Get the cell controller of the hit bubble
			var cellController = bubbleHit.collider.GetComponent<BubbleController>().cellController;

			// If there's no cell controller, return null
			if (ReferenceEquals(cellController, null))
				return rayPath;

			_lastHitPoint = bubbleHit.point;

			// Get the free direction from the hit cell
			var targetCellDirection = GetFreeDirectionFromHitCell(bubbleHit.normal, cellController);

			if (!targetCellDirection.HasValue)
				return rayPath;

			var neighbourPos = cellController.Neighbours.GetNeighbour(targetCellDirection.Value)?.transform.position;
			if (neighbourPos.HasValue)
				rayPath.Add(neighbourPos.Value);

			lastTargetCell = cellController.Neighbours.GetNeighbour(targetCellDirection.Value);
			lastTargetCellDirection = targetCellDirection.Value;
			return rayPath;
		}

		private Vector2 GetMousePosition(Vector2 casterPosition)
		{
			var mousePos = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
			var delta = mousePos - casterPosition;
			return delta;
		}

		private Direction? GetFreeDirectionFromHitCell(Vector2 normal, CellController hitCell)
		{
			// Check which neighboring cells are free.
			bool rightCellIsFree = IsCellFree(hitCell, Direction.Right);
			bool leftCellIsFree = IsCellFree(hitCell, Direction.Left);
			bool downRightCellIsFree = IsCellFree(hitCell, Direction.DownRight);
			bool downLeftCellIsFree = IsCellFree(hitCell, Direction.DownLeft);
			bool upperRightCellIsFree = IsCellFree(hitCell, Direction.UpRight);
			bool upperLeftCellIsFree = IsCellFree(hitCell, Direction.UpLeft);

			// Determine the direction to move in based on the normal vector and free neighboring cells.
			Direction? direction;

			switch (normal.y)
			{
				// If the hit was from below the bubble...
				case < 0:
					// Try to get exact direction (right, down-right, left, or down-left)
					direction = normal.x switch
					{
						> 0 =>
							downRightCellIsFree ? Direction.DownRight :
							rightCellIsFree ? Direction.Right :
							downLeftCellIsFree ? Direction.DownLeft :
							leftCellIsFree ? Direction.Left : null,
						_ =>
							downLeftCellIsFree ? Direction.DownLeft :
							leftCellIsFree ? Direction.Left :
							downRightCellIsFree ? Direction.DownRight :
							rightCellIsFree ? Direction.Right : null
					};
					break;
				// If the hit was from above the bubble...
				case > 0:
					// Try to get exact direction (right, up-right, left, or up-left)
					direction = normal.x switch
					{
						> 0 =>
							upperRightCellIsFree ? Direction.UpRight :
							rightCellIsFree ? Direction.Right :
							upperLeftCellIsFree ? Direction.UpLeft :
							leftCellIsFree ? Direction.Left : null,
						_ =>
							upperLeftCellIsFree ? Direction.UpLeft :
							leftCellIsFree ? Direction.Left :
							upperRightCellIsFree ? Direction.UpRight :
							rightCellIsFree ? Direction.Right : null
					};
					break;
				// If the hit was from the side of the bubble...
				default:
					// Try to get exact direction (right or left)
					direction = normal.x > 0 ? Direction.Right : Direction.Left;
					break;
			}

			return direction;
		}

		// Returns true if a neighboring cell in the specified direction is free.
		private bool IsCellFree(CellController cellController, Direction direction)
		{
			var neighbourCell = cellController.Neighbours.GetNeighbour(direction);
			var cellExists = !ReferenceEquals(neighbourCell, null);
			return cellExists && neighbourCell.bubbleController == null;
		}

		//Set line renderer and bubble indicator
		private void SetVisuals(List<Vector3> rayPath)
		{
			if (ReferenceEquals(rayPath, null) || rayPath.Count == 0)
			{
				SetLine(new List<Vector3>());
				bubbleIndicator.enabled = false;
			}
			else
			{
				if (_lastIndicatorPos != rayPath[^1])
				{
					DOTween.Kill(bubbleIndicator.transform);
					bubbleIndicator.transform.localScale = Vector3.zero;
					bubbleIndicator.transform.DOScale(Vector3.one, 0.3f);
					_lastIndicatorPos = rayPath[^1];
				}

				bubbleIndicator.enabled = true;
				bubbleIndicator.transform.position = rayPath[^1];

				SetLine(rayPath);
			}

			
		}

		private void SetLine(List<Vector3> rayPath)
		{
			lineRenderer.positionCount = rayPath.Count;
			if (rayPath.Count <= 0) return;

			lineRenderer.SetPositions(rayPath.ToArray());
			lineRenderer.SetPosition(lineRenderer.positionCount - 1, _lastHitPoint);
		}

		public void SetBubblePredictionColor(Color color)
		{
			bubbleIndicator.color = color.With(a:0.5f);
		}
	}
}