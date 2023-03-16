using System.Collections.Generic;
using DG.Tweening;
using Extensions.Vectors;
using GridSystem;
using Pooling;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BubbleSystem
{
	public class BubbleShooterManager : Singleton<BubbleShooterManager>
	{
		#region References

		[BoxGroup("References"), SerializeField]
		private BubbleShootPredictionHandler predictionHandler;

		[BoxGroup("References"), SerializeField]
		private Transform shooterTransform;

		[BoxGroup("References"), SerializeField]
		private List<BubbleController> bubbleList;

		private BubbleController CurrentBubble => bubbleList[0];

		#endregion

		#region Variables

		[BoxGroup("Variables"), SerializeField]
		private float secondBubbleOffset = -1.15f;

		[BoxGroup("Variables"), SerializeField]
		private float secondBubbleScale = 0.75f;

		[BoxGroup("Variables"), SerializeField]
		private float moveSpeed = 1f;

		private bool _isBubbleReady = true;

		#endregion

		public void PrepareShooter()
		{
			for (int i = 0; i < 2; i++)
			{
				CreateNewBubble();
			}

			UpdatePredictionColor();
		}

		public void OnPointerDown()
		{
			if(!TouchManager.CanTouch)
				return;
			TouchManager.TouchCount++;
			
			if(!GameManager.Instance.CanTouch)
				return;
			
			if(!_isBubbleReady)
				return;
			predictionHandler.OnMouseButton();
		}

		public void OnDrag()
		{
			if(!GameManager.Instance.CanTouch)
				return;
			if(TouchManager.TouchCount==1 && _isBubbleReady)
				predictionHandler.OnMouseButton();
		}
		
		public void OnPointerUp()
		{
			TouchManager.TouchCount--;
			if(!GameManager.Instance.CanTouch)
				return;
			if(!_isBubbleReady)
				return;
			_isBubbleReady = false;
			GameManager.Instance.CanTouch = false;
			predictionHandler.OnMouseButtonUp();
			ShootBubble();
		}

		public void ShootBubble()
		{
			var targetCell = predictionHandler.lastTargetCell;
			if (ReferenceEquals(targetCell, null))
			{
				GameManager.Instance.CanTouch = true;
				_isBubbleReady = true;
				return;
			}

			
			var bubbleController = CurrentBubble;
			targetCell.bubbleController = bubbleController;
			bubbleController.transform.SetParent(targetCell.transform, true);
			bubbleController.cellController = targetCell;
			bubbleController.bubbleTrail.enabled = true;
			bubbleController.bubbleCollider.enabled = true;
			bubbleList.RemoveAt(0);

			var path = predictionHandler.lastRayPath;
			path.RemoveAt(0);
			path[^1] = targetCell.transform.position;
			BubbleManager.Instance.lastShotBubble = bubbleController;
			bubbleController.transform.DOPath(path.ToArray(), moveSpeed)
				.SetEase(Ease.Linear)
				.SetSpeedBased(true)
				.OnComplete(() =>
				{
					bubbleController.bubbleTrail.enabled = false;
					EquipSecondBubble();
					Reload();
					targetCell.ShootEffectToNeighbours();

					predictionHandler.lastRayPath.Clear();
					predictionHandler.lastTargetCell = null;
					BubbleManager.Instance.onBubbleHit.Invoke();
				});
		}

		private void Reload()
		{
			while (bubbleList.Count != 2)
			{
				CreateNewBubble();
			}
		}

		private void EquipSecondBubble()
		{
			var bubbleController = CurrentBubble;
			bubbleController.transform.DOScale(Vector3.one, 0.5f);
			bubbleController.transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete((() => _isBubbleReady = true));
			UpdatePredictionColor();
		}

		private void CreateNewBubble(bool animated = false)
		{
			var data = BubbleManager.Instance.bubbleDataPoolHandler.GetBubbleDataFromPool();
			var bubble = PoolingManager.Instance.GetObjectFromPool("Bubble");
			var bubbleController = bubble.GetComponent<BubbleController>();
			
			bubbleController.ResetBubble();
			
			bubbleList.Add(bubbleController);
			bubbleController.bubbleCollider.enabled = false;
			bubbleController.SetData(data);
			bubble.transform.SetParent(shooterTransform, false);

			var isSecond = bubbleList.Count == 2;
			var offset = isSecond ? secondBubbleOffset : 0f;
			var scale = isSecond ? secondBubbleScale : 1f;
			bubble.transform.localPosition = Vector3.zero.WithAddX(offset);
			bubble.transform.localScale = Vector3.one * scale;
		}

		private void UpdatePredictionColor()
		{
			predictionHandler.SetBubblePredictionColor(CurrentBubble.data.color);
		}
	}
}