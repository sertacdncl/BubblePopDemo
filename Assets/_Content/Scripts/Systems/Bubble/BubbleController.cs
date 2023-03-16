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

		[TabGroup("References")] [SerializeField]
		public SpriteRenderer bubbleSprite;

		[TabGroup("References")] public CircleCollider2D bubbleCollider;

		[TabGroup("References")] public Rigidbody2D bubbleRigidbody;

		[TabGroup("References")] public TrailRenderer bubbleTrail;

		#endregion

		#region Variables

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