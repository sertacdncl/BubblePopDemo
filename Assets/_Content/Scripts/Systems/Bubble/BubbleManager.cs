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
		[BoxGroup("References")] public SpriteRenderer explodeEffectSRenderer;

		[BoxGroup("References"), ReadOnly] public BubbleController lastShotBubble;
		[BoxGroup("References"), ReadOnly] public List<BubbleController> needExplodeList;

		private List<BubbleController> _possibleFallBubbleList = new();

		#endregion

		#region Variables

		private bool _explodeRunning;
		private bool _mergeRunning;
		private int _nonStopMergeCount = 0;

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
				if (ReferenceEquals(cellController.bubbleController, null))
					continue;
				cellController.bubbleController.connectedBubbles.Clear();
			}

			for (int x = 0; x < GridManager.Instance.GridLength.x; x++)
			{
				if (ReferenceEquals(GridManager.Instance.CellController[x, GridManager.Instance.GridLength.y - 1], null))
					continue;
				if (ReferenceEquals(
						GridManager.Instance.CellController[x, GridManager.Instance.GridLength.y - 1].bubbleController,
						null))
					continue;
				ConnectBubbles(
					GridManager.Instance.CellController[x, GridManager.Instance.GridLength.y - 1].bubbleController);
			}
		}

		private void ConnectBubbles(BubbleController startingBubble, List<BubbleController> connectedBubbles = null)
		{
			var cellController = startingBubble.cellController;
			var directions = new List<Direction>
				{ Direction.Right, Direction.Left, Direction.DownRight, Direction.DownLeft };

			foreach (var direction in directions)
			{
				var neighbourCell = cellController.Neighbours.GetNeighbour(direction);
				if (ReferenceEquals(neighbourCell, null))
					continue;

				if (neighbourCell.coordinate.y >= GridManager.Instance.GridLength.y - 1)
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

		private void CreateBubbleToCell(CellController cellController, bool animated = false)
		{
			//Getting bubble object from pool
			GameObject bubble = PoolingManager.Instance.GetObjectFromPool("Bubble");
			var bubbleController = bubble.GetComponent<BubbleController>();

			//Setting bubble defaults
			bubbleController.ResetBubble(true);

			//Setting bubble parent, reference and name
			cellController.bubbleController = bubbleController;
			bubbleController.cellController = cellController;
			bubble.transform.SetParent(cellController.transform, false);
			bubble.name = $"Bubble";

			//Setting bubble data
			var bubbleData = bubbleDataPoolHandler.GetBubbleDataFromPool();
			bubbleController.SetData(bubbleData);
			if (animated)
			{
				bubbleController.transform.localScale = Vector3.zero;
				bubbleController.transform.DOScale(Vector3.one, 0.5f);
			}
		}

		[Button]
		public void CreateBubblesToRow(int rowCoord, bool animated = false)
		{
			for (int x = 0; x < GridManager.Instance.GridLength.x; x++)
			{
				var cellController = GridManager.Instance.CellController[x, rowCoord];

				CreateBubbleToCell(cellController, animated);
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

			if (_nonStopMergeCount > 1)
				PopupTextManager.Instance.ShowComboCounterText(_nonStopMergeCount);
			_nonStopMergeCount = 0;
		}


		#region Merge Methods

		private void DoMergeIfAvailable(BubbleController bubble)
		{
			var matchedBubbleList = new List<BubbleController>();
			if (ReferenceEquals(bubble?.cellController, null))
			{
				onMergeComplete.Invoke();
				return;
			}

			GetMatchedBubbles(bubble, matchedBubbleList);

			if (matchedBubbleList.Count < 2)
			{
				onMergeComplete.Invoke();
				return;
			}

			_mergeRunning = true;

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

				CheckPossibleFallBubbleList(bubbleController);

				bubbleController.cellController.bubbleController = null;
				bubbleController.cellController = null;
				bubbleController.transform.DOMove(mergeBubblePos, 10f).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(
					() =>
					{
						EmitDestroyParticle(mergeBubble);
						PoolingManager.Instance.ReturnObjectToPool(bubbleController.gameObject, "Bubble");
						bubbleController.ResetBubble();
						finishedTweenCount++;
						if (finishedTweenCount == matchedBubbleList.Count)
							_mergeRunning = false;
					});
			}

			AudioManager.Instance.PlaySoundOnce("BubbleMerge",true);
			StartCoroutine(WaitForMergeComplete(mergeBubble, matchedBubbleList, afterMergeValue, finishedTweenCount));
		}

		private IEnumerator WaitForMergeComplete(BubbleController mergeBubble, List<BubbleController> matchedBubbleList,
			int afterMergeValue, int finishedTweenCount)
		{
			yield return new WaitUntil(() => _mergeRunning == false);

			//Update bubble data and animate
			var bubbleDataFromValue = bubbleDataPoolHandler.GetBubbleDataFromValue(afterMergeValue);
			mergeBubble.SetDataScaleAnimated(bubbleDataFromValue);

			//Combo counter
			_nonStopMergeCount++;
			//Showing popup texts
			if (_nonStopMergeCount > 1)
				PopupTextManager.Instance.ShowComboCounterText(_nonStopMergeCount);
			PopupTextManager.Instance.ShowPopupText(afterMergeValue, mergeBubble.transform.position);
			var score = _nonStopMergeCount > 0 ? afterMergeValue * _nonStopMergeCount : afterMergeValue;
			ScoreManager.Instance.UpdateScore(score);


			//Check falling bubbles
			DoFallIfNeeded();

			//Waiting for after merge scale animation
			yield return new WaitForSeconds(0.3f);

			if (needExplodeList.Count > 0)
			{
				foreach (var bubbleController in needExplodeList)
				{
					Explode(bubbleController);
				}

				needExplodeList.Clear();

				yield return new WaitUntil(() => _explodeRunning == false);

				//Check falling bubbles
				DoFallIfNeeded();
				onMergeComplete.Invoke();
			}
			else
			{
				if (!ReferenceEquals(mergeBubble, null))
				{
					if (mergeBubble.bubbleRigidbody.bodyType == RigidbodyType2D.Static)
						DoMergeIfAvailable(mergeBubble);
					else
						onMergeComplete.Invoke();
				}
				else
					onMergeComplete.Invoke();
			}
		}

		private void DoFallIfNeeded()
		{
			UpdateConnectedBubbles();
			if (_possibleFallBubbleList.Count > 0)
			{
				foreach (var bubbleController in _possibleFallBubbleList)
				{
					FallBubbleIfDisconnected(bubbleController);
				}

				_possibleFallBubbleList.Clear();
			}

			UpdateConnectedBubbles();
		}

		private void EmitDestroyParticle(BubbleController mergeBubble, int emitCount = 10)
		{
			var particleParams = new ParticleSystem.EmitParams
			{
				position = mergeBubble.transform.position,
				startColor = mergeBubble.data.color,
				applyShapeToPosition = true
			};
			destroyParticle.Emit(particleParams, emitCount);
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
				var bubbleCoord = bubble.cellController.coordinate;
				hightestBubble = hightestBubble ?? bubble;
				var highestBubbleCoord = hightestBubble.cellController.coordinate;

				//If bubble is higher than highest bubble, set it as highest bubble
				if (bubbleCoord.y >= highestBubbleCoord.y &&
					(bubbleCoord.y != highestBubbleCoord.y || bubbleCoord.x >= highestBubbleCoord.x) && bubble.IsConnected)
				{
					hightestBubble = bubble;
				}

				//If neighbours of bubble has mergeable value after merge increase mergeable neighbour variable
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

		#endregion

		private void CheckPossibleFallBubbleList(BubbleController bubbleController)
		{
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

				if (!_possibleFallBubbleList.Contains(neighbourCell.bubbleController))
				{
					_possibleFallBubbleList.Add(neighbourCell.bubbleController);
					CheckPossibleFallBubbleList(neighbourCell.bubbleController);
				}
			}
		}

		private void Explode(BubbleController bubble)
		{
			//This is for prevent bug
			if(ReferenceEquals(bubble, null) || bubble.IsFalling)
				return;
			_explodeRunning = true;
			foreach (var neighbourCell in bubble.cellController.Neighbours.GetAllNeighbour())
			{
				if (ReferenceEquals(neighbourCell?.bubbleController, null))
					continue;

				CheckPossibleFallBubbleList(neighbourCell.bubbleController);
				var neighbourBubble = neighbourCell.bubbleController;
				neighbourBubble.Fall();
				neighbourBubble.transform.DOScale(Vector3.zero, .5f).OnComplete((() =>
				{
					PoolingManager.Instance.ReturnObjectToPool(neighbourBubble.gameObject, "Bubble");
					neighbourBubble.ResetBubble();
				}));
			}

			EmitDestroyParticle(bubble, 30);
			CameraShaker.Instance.ShakeCameraOnExplode();
			AudioManager.Instance.PlaySoundOnce("BubbleExplode");
			bubble.cellController.bubbleController = null;
			bubble.cellController = null;
			bubble.bubbleShadowSprite.enabled = false;
			explodeEffectSRenderer.transform.position = bubble.transform.position;
			explodeEffectSRenderer.enabled = true;
			explodeEffectSRenderer.transform.DOScale(Vector3.one * 2f, 0.5f).OnComplete(() =>
			{
				explodeEffectSRenderer.enabled = false;
				explodeEffectSRenderer.transform.localScale = Vector3.one * 0.75f;
			});
			Sequence sequence = DOTween.Sequence();
			sequence.Append(bubble.transform.DOScale(Vector3.one * 1.5f, 0.5f));
			sequence.Join(bubble.bubbleSprite.DOFade(0, 0.5f));
			sequence.OnComplete(() =>
			{
				PoolingManager.Instance.ReturnObjectToPool(bubble.gameObject, "Bubble");
				bubble.ResetBubble();
				_explodeRunning = false;
			});
		}

		private void FallBubbleIfDisconnected(BubbleController bubbleController)
		{
			if (ReferenceEquals(bubbleController, null))
				return;
			if (ReferenceEquals(bubbleController.cellController, null))
				return;
			if (bubbleController.cellController.coordinate.y >= GridManager.Instance.GridLength.y - 1)
				return;

			var needToFall = true;
			for (int x = 0; x < GridManager.Instance.GridLength.x; x++)
			{
				var topCell = GridManager.Instance.GetCell(x, GridManager.Instance.GridLength.y - 1);

				if (ReferenceEquals(topCell, null))
					continue;

				if (ReferenceEquals(topCell.bubbleController, null))
					continue;
				
				var isMyNeighbourConnected = false;
				foreach (var neighbour in bubbleController.cellController.Neighbours.GetAllNeighbour())
				{
					//Checking fall neededs
					if (ReferenceEquals(neighbour, null))
						continue;
					
					if (neighbour.coordinate.y == GridManager.Instance.GridLength.y - 1)
						continue;

					if (ReferenceEquals(neighbour.bubbleController, null))
						continue;

					if (topCell.bubbleController.connectedBubbles.Contains(neighbour.bubbleController))
					{
						isMyNeighbourConnected = true;
						break;
					}
				}

				if (topCell.bubbleController.connectedBubbles.Contains(bubbleController) || isMyNeighbourConnected)
					return;
				
			}

			if (needToFall)
			{
				bubbleController.Fall();
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
			bubbleController.ResetBubble();
		}
	}
}