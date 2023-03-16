using Pooling;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PopupTextSystem
{
	public class PopupTextManager : Singleton<PopupTextManager>
	{

		#region Refere

		[BoxGroup("References"), SerializeField]
		private Camera mainCam;
		
		#endregion
		#region Variables

		[SerializeField] private float comboCounterTextSize = 15f;
		[SerializeField] private float perfectTextSize = 15f;

		#endregion

		public void ShowPopupText(int value, Vector3 position)
		{
			var popupText = PoolingManager.Instance.GetObjectFromPool("PopupText");
			var popupTextController = popupText.GetComponent<PopupTextController>();
			popupTextController.textMesh.text = value.ToString();
			popupText.transform.position = position;
			popupTextController.Show();
		}
		
		public void ShowPerfectText()
		{
			var popupText = PoolingManager.Instance.GetObjectFromPool("PerfectText");
			var position = mainCam.ViewportToWorldPoint(Vector3.one * .5f);
			popupText.transform.position = position;
			var popupTextController = popupText.GetComponent<PopupTextController>();
			popupTextController.textMesh.fontSize = perfectTextSize;
			popupTextController.textMesh.text = "Perfect";
		}
		
		public void ShowComboCounterText(int comboCount)
		{
			var popupText = PoolingManager.Instance.GetObjectFromPool("ComboCounterText");
			var position = mainCam.ViewportToWorldPoint(Vector3.one * .5f);
			popupText.transform.position = position;
			var popupTextController = popupText.GetComponent<PopupTextController>();
			popupTextController.textMesh.text = "x" + comboCount;
		}
	}
}