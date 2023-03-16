using System.Collections.Generic;
using DG.Tweening;
using GridSystem;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace BubbleSystem
{
	public class BubbleController : MonoBehaviour
	{
		#region Reference

		[TabGroup("References"), ReadOnly] public CellController cellController;

		[TabGroup("References")] public BubbleData data;

		[TabGroup("References")] public List<BubbleController> connectedBubbles;

		[TabGroup("References")] [SerializeField]
		private TextMeshPro valueText;

		[TabGroup("References")]
		public SpriteRenderer bubbleSprite;
		
		[TabGroup("References")]
		public SpriteRenderer bubbleShadowSprite;

		[TabGroup("References")] public CircleCollider2D bubbleCollider;

		[TabGroup("References")] public Rigidbody2D bubbleRigidbody;

		[TabGroup("References")] public TrailRenderer bubbleTrail;

		#endregion

		#region Variables

		public bool IsFalling => bubbleRigidbody.bodyType == RigidbodyType2D.Dynamic;
		public bool IsConnected
		{
			get
			{
				var topRow = GridManager.Instance.GridLength.y - 1;
				var columnCount = GridManager.Instance.GridLength.x;
				var isConnected = false;
				for (int x = 0; x < columnCount; x++)
				{
					var cell = GridManager.Instance.GetCell(x, topRow);
					if(ReferenceEquals(cell?.bubbleController, null))
						continue;
					if (cell.bubbleController.connectedBubbles.Contains(this))
					{
						isConnected = true;
						break;
					}
				}

				return isConnected;
			}
		}

		#endregion

		#region Properties

		#endregion

		[Button]
		private void ShowConnectedBubbles()
		{
			foreach (var connectedBubble in connectedBubbles)
			{
				connectedBubble.transform.DOPunchScale(0.2f * Vector3.one, 1f);
			}
		}

		public void ResetBubble(bool overrideScale = false)
		{
			//Setting bubble defaults
			bubbleRigidbody.bodyType = RigidbodyType2D.Static;
			bubbleRigidbody.velocity = Vector2.zero;
			bubbleShadowSprite.enabled = true;

			//Setting bubble position and rotation
			transform.localPosition = Vector3.zero;
			if(!overrideScale)
				transform.localScale = Vector3.one;
			transform.rotation = Quaternion.identity;
		}
		
		public void SetData(BubbleData bubbleData)
		{
			data = bubbleData;
			valueText.text = data.value.ToString();
			bubbleSprite.color = data.color;
		}

		public void SetDataScaleAnimated(BubbleData bubbleData)
		{
			data = bubbleData;
			transform.DOScale(Vector3.zero, 0.1f).OnComplete(() =>
			{
				valueText.text = data.value.ToString();
				bubbleSprite.color = data.color;
				transform.DOScale(Vector3.one, 0.1f);
			});
		}
		
	}
}