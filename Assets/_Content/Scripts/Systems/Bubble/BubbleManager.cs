using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GridSystem;
using Pooling;
using PopupTextSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace BubbleSystem
{
	public class BubbleManager : Singleton<BubbleManager>
	{
		#region References

		[BoxGroup("References")] public BubbleDataPoolHandler bubbleDataPoolHandler;
		[BoxGroup("References")] public ParticleSystem destroyParticle;

		[BoxGroup("References"), ReadOnly] public BubbleController lastShotBubble;
		[BoxGroup("References"), ReadOnly] public List<BubbleController> needExplodeList;

		#endregion

		#region Variables

		#endregion

		#region Events

		[HideInInspector] public UnityEvent onBubbleHit = new();
		[HideInInspector] public UnityEvent onMergeComplete = new();

		#endregion

		private void OnEnable()
		{
			onBubbleHit.AddListener(OnBubbleHit);
			onMergeComplete.AddListener(OnMergeComplete);
		}

		private void OnDisable()
		{
			onBubbleHit.RemoveListener(OnBubbleHit);
			onMergeComplete.RemoveListener(OnMergeComplete);
		}

		public void PrepareStart()
		{
			for (int y = 1; y < GridManager.Instance.GridLength.y; y++)
			{
				for (int x = 0; x < GridManager.Instance.GridLength.x; x++)
				{
					//Target cell
					var cellController = GridManager.Instance.CellController[x, y];

					CreateBubbleToCell(cellController);
				}
			}

			//To find all connected bubbles to top row
			UpdateConnectedBubbles();
		}

		public void UpdateConnectedBubbles()
		{
			foreach (var cellController in GridManager.Instance.cellControllerList)
			{
				if(ReferenceEquals(cellController.bubbleController, null))
					continue;
				cellController.bubbleController.connectedBubbles.Clear();
			}
			for (int x = 0; x < GridManager.Instance.GridLength.x; x++)
			{
				if(ReferenceEquals(GridManager.Instance.CellController[x, GridManager.Instance.GridLength.y - 1], null))
					continue;
				if(ReferenceEquals(GridManager.Instance.CellController[x, GridManager.Instance.GridLength.y - 1].bubbleController, null))
					continue;
				ConnectBubbles(GridManager.Instance.CellController[x, GridManager.Instance.GridLength.y - 1].bubbleController);
			}
		}

		private void ConnectBubbles(BubbleController startingBubble, List<BubbleController> connectedBubbles = null)
		{
			var cellController = startingBubble.cellController;
			var directions = new List<Direction> {Direction.Right, Direction.Left, Direction.DownRight, Direction.DownLeft};
				
			foreach (var direction in directions)
			{
				var neighbourCell = cellController.Neighbours.GetNeighbour(direction);
				if (ReferenceEquals(neighbourCell, null))
					continue;

				if(neighbourCell.coordinate.y >= GridManager.Instance.GridLength.y-1)
					continue;
					
				if (ReferenceEquals(neighbourCell.bubbleController, null))
					continue;

				//If connected bubbles is null, it means that we will use starting bubble because bubble at top row
				if (ReferenceEquals(connectedBubbles, null))
				{
					if (!startingBubble.connectedBubbles.Contains(neighbourCell.bubbleController))
					{
						startingBubble.connectedBubbles.Add(neighbourCell.bubbleController);
						ConnectBubbles(neighbourCell.bubbleController, startingBubble.connectedBubbles);
					}
				}
				else
				{
					if (!connectedBubbles.Contains(neighbourCell.bubbleController))
					{
						connectedBubbles.Add(neighbourCell.bubbleController);
						ConnectBubbles(neighbourCell.bubbleController, connectedBubbles);
					}
				}
			}
		}

		private void CreateBubbleToCell(CellController cellController)
		{
			//Getting bubble object from pool
			GameObject bubble = PoolingManager.Instance.GetObjectFromPool("Bubble");
			var bubbleController = bubble.GetComponent<BubbleController>();

			//Setting bubble position and rotation
			bubble.transform.localPosition = Vector3.zero;
			bubble.transform.rotation = Quaternion.identity;

			//Setting bubble parent, reference and name
			cellController.bubbleController = bubbleController;
			bubbleController.cellController = cellController;
			bubble.transform.SetParent(cellController.transform, false);
			bubble.name = $"Bubble";

			//Setting bubble data
			var bubbleData = bubbleDataPoolHandler.GetBubbleDataFromPool();
			bubbleController.SetData(bubbleData);
		}

		[Button]
		public void CreateBubbleRow(int rowCoord)
		{
			for (int x = 0; x < GridManager.Instance.GridLength.x; x++)
			{
				var cellController = GridManager.Instance.CellController[x, rowCoord];

				CreateBubbleToCell(cellController);
			}
		}

		private void OnBubbleHit()
		{
			var bubble = lastShotBubble;
			DoMergeIfAvailable(bubble);
		}

		private void OnMergeComplete()
		{
			GridManager.Instance.CheckGridAndProcess();
			GameManager.Instance.CanTouch = true;
		}

		private void Explode(BubbleController bubble)
		{
			foreach (var neighbourCell in bubble.cellController.Neighbours.GetAllNeighbour())
			{
				if (ReferenceEquals(neighbourCell.bubbleController, null))
					continue;

				var neighbourBubble = neighbourCell.bubbleController;
				neighbourBubble.cellController.bubbleController = null;
				neighbourBubble.cellController = null;
				neighbourBubble.bubbleRigidbody.bodyType = RigidbodyType2D.Dynamic;
				neighbourBubble.bubbleRigidbody.AddForce(Vector2.right * Random.Range(-2f, 2f), ForceMode2D.Impulse);
				neighbourBubble.transform.DOScale(Vector3.zero, .5f).OnComplete((() =>
				{
					PoolingManager.Instance.ReturnObjectToPool(neighbourBubble.gameObject, "Bubble");
				}));
			}

			var particleParams = new ParticleSystem.EmitParams
			{
				position = bubble.transform.position,
				startColor = bubble.data.color,
				applyShapeToPosition = true
			};
			destroyParticle.Emit(particleParams, 30);
			bubble.cellController.bubbleController = null;
			bubble.cellController = null;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(bubble.transform.DOScale(Vector3.one * 1.5f, 0.5f));
			sequence.Join(bubble.bubbleSprite.DOFade(0, 0.5f));
			sequence.OnComplete(() => { PoolingManager.Instance.ReturnObjectToPool(bubble.gameObject, "Bubble"); });
		}

		
		List<BubbleController> possibleFallBubbleList = new();
		private bool DoMergeIfAvailable(BubbleController bubble)
		{
			var matchedBubbleList = new List<BubbleController>();
			GetMatchedBubbles(bubble, matchedBubbleList);

			if (matchedBubbleList.Count < 2)
			{
				onMergeComplete.Invoke();
				return false;
			}

			var afterMergeValue = bubble.data.value * (1 << (matchedBubbleList.Count - 1));
			if (afterMergeValue > 2048)
				afterMergeValue = 2048;

			var mergeBubble = GetBestMergeBubble(matchedBubbleList, afterMergeValue);
			var mergeBubblePos = mergeBubble.transform.position;
			if (afterMergeValue == 2048)
				needExplodeList.Add(mergeBubble);
			matchedBubbleList.Remove(mergeBubble);
			
			var finishedTweenCount = 0;
			for (var index = 0; index < matchedBubbleList.Count; index++)
			{
				var bubbleController = matchedBubbleList[index];

				foreach (var neighbourCell in bubbleController.cellController.Neighbours.GetAllNeighbour())
				{
					//Checking fall neededs
					if (ReferenceEquals(neighbourCell, null))
						continue;

					if (neighbourCell.coordinate.y == GridManager.Instance.GridLength.y - 1)
						continue;
					
					if (ReferenceEquals(neighbourCell.bubbleController, null))
						continue;
					
					if (neighbourCell.bubbleController.data.value == 2048)
						continue;
					
					if(!possibleFallBubbleList.Contains(neighbourCell.bubbleController))
						possibleFallBubbleList.Add(neighbourCell.bubbleController);

				}
				
				bubbleController.cellController.bubbleController = null;
				bubbleController.cellController = null;
				bubbleController.transform.DOMove(mergeBubblePos, 10f).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(
					() =>
					{
						var particleParams = new ParticleSystem.EmitParams
						{
							position = mergeBubble.transform.position,
							startColor = mergeBubble.data.color,
							applyShapeToPosition = true
						};
						destroyParticle.Emit(particleParams, 10);
						PoolingManager.Instance.ReturnObjectToPool(bubbleController.gameObject, "Bubble");

						finishedTweenCount++;
					});
			}

			StartCoroutine(WaitingForFinish());

			IEnumerator WaitingForFinish()
			{
				yield return new WaitUntil((() => finishedTweenCount == matchedBubbleList.Count));
				var bubbleDataFromValue = bubbleDataPoolHandler.GetBubbleDataFromValue(afterMergeValue);
				mergeBubble.SetDataScaleAnimated(bubbleDataFromValue);
				PopupTextManager.Instance.ShowPopupText(afterMergeValue, mergeBubble.transform.position);
				UpdateConnectedBubbles();
				if (possibleFallBubbleList.Count > 0)
				{
					foreach (var bubbleController in possibleFallBubbleList)
					{
						CheckBubbleDisconnected(bubbleController);
					}
				}

				if (needExplodeList.Count > 0)
				{
					DOVirtual.DelayedCall(0.3f, () =>
					{
						foreach (var bubbleController in needExplodeList)
						{
							Explode(bubbleController);
						}

						needExplodeList.Clear();
						DOVirtual.DelayedCall(0.7f, () => DoMergeIfAvailable(mergeBubble));
					});
				}
				else
					DOVirtual.DelayedCall(0.3f, () => DoMergeIfAvailable(mergeBubble));
			}


			return true;
		}

		private void GetMatchedBubbles(BubbleController bubbleController, List<BubbleController> matchedList)
		{
			var bubbleValue = bubbleController.data.value;
			foreach (var neighbourCell in bubbleController.cellController.Neighbours.GetAllNeighbour())
			{
				if (ReferenceEquals(neighbourCell?.bubbleController, null))
					continue;

				if (neighbourCell.bubbleController.data.value == bubbleValue &&
					!matchedList.Contains(neighbourCell.bubbleController))
				{
					matchedList.Add(neighbourCell.bubbleController);
					GetMatchedBubbles(neighbourCell.bubbleController, matchedList);
				}
			}
		}

		private BubbleController GetBestMergeBubble(List<BubbleController> mergeList, int afterMergeValue)
		{
			var mergeableNeighbourCount = 0;
			BubbleController bestBubble = null;
			BubbleController hightestBubble = null;

			foreach (var bubble in mergeList)
			{
				var bubblePos = bubble.cellController.coordinate;
				hightestBubble = hightestBubble ?? bubble;
				var highestBubblePos = hightestBubble.cellController.coordinate;

				if (bubblePos.y >= highestBubblePos.y &&
					(bubblePos.y != highestBubblePos.y || bubblePos.x >= highestBubblePos.x))
				{
					hightestBubble = bubble;
				}

				var mergeableNeighbours = 0;
				foreach (var neighbourCell in bubble.cellController.Neighbours.GetAllNeighbour())
				{
					if (ReferenceEquals(neighbourCell?.bubbleController, null))
						continue;

					if (neighbourCell.bubbleController.data.value == afterMergeValue)
						mergeableNeighbours++;
				}

				if (mergeableNeighbours > mergeableNeighbourCount)
				{
					bestBubble = bubble;
					mergeableNeighbourCount = mergeableNeighbours;
				}
			}

			return bestBubble ?? hightestBubble;
		}

		public void CheckBubbleDisconnected(BubbleController bubbleController)
		{
			if(ReferenceEquals(bubbleController,null))
				return;
			if(ReferenceEquals(bubbleController.cellController,null))
				return;
			if(bubbleController.cellController.coordinate.y >= GridManager.Instance.GridLength.y-1)
				return;

			var needToFall = true;
			for (int x = 0; x < GridManager.Instance.GridLength.x; x++)
			{
				var topCell = GridManager.Instance.GetCell(x, GridManager.Instance.GridLength.y-1);
				
				if(ReferenceEquals(topCell,null))
					continue;
				
				if(ReferenceEquals(topCell.bubbleController,null))
					continue;

				if (topCell.bubbleController.connectedBubbles.Contains(bubbleController))
				{
					needToFall = false;
					return;
				}
			}

			if (needToFall)
			{
				bubbleController.cellController.bubbleController = null;
				bubbleController.cellController = null;
				bubbleController.bubbleRigidbody.bodyType = RigidbodyType2D.Dynamic;
				bubbleController.bubbleRigidbody.AddForce(Vector2.right * Random.Range(-2f, 2f), ForceMode2D.Impulse);		
			}
		}

		public void DestroyBubbleWithParticle(BubbleController bubbleController)
		{
			//This part is for falling bubbles
			var particleParams = new ParticleSystem.EmitParams
			{
				position = bubbleController.transform.position,
				startColor = bubbleController.data.color,
				applyShapeToPosition = true
			};
			destroyParticle.Emit(particleParams, 10);
			PoolingManager.Instance.ReturnObjectToPool(bubbleController.gameObject, "Bubble");
		}
	}
}