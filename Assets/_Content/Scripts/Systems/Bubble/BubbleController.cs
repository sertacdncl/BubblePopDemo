using GridSystem;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace BubbleSystem
{
	public class BubbleController : MonoBehaviour
	{
		#region Reference

		[TabGroup("References"), ReadOnly] 
		public CellController cellController;

		[TabGroup("References")] 
		public BubbleData data;

		[TabGroup("References")] [SerializeField]
		private TextMeshPro valueText;

		[TabGroup("References")] [SerializeField]
		private SpriteRenderer bubbleSprite;
		
		[TabGroup("References")]
		public CircleCollider2D bubbleCollider;
		
		[TabGroup("References")]
		public Rigidbody2D bubbleRigidbody;

		[TabGroup("References")]
		public TrailRenderer bubbleTrail;
		
		#endregion

		#region Variables

		#endregion

		#region Properties

		#endregion

		public void SetData(BubbleData bubbleData)
		{
			data = bubbleData;
			valueText.text = data.value.ToString();
			bubbleSprite.color = data.color;
		}
	}
}