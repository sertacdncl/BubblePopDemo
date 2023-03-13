using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace BubbleSystem
{
	public class BubbleController : MonoBehaviour
	{
		#region Reference

		[TabGroup("References")] [SerializeField]
		private TextMeshPro valueText;

		[TabGroup("References")] public BubbleData data;

		[TabGroup("References")] [SerializeField]
		private SpriteRenderer bubbleSprite;

		[TabGroup("References")] [SerializeField]
		private TextMeshPro bubbleValueText;

		#endregion

		#region Variables

		#endregion

		#region Properties

		#endregion
	}
}