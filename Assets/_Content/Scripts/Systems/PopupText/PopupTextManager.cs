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
		
		private PopupTextController _activeComboCounterPopup;
		
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
			return;
			var popupText = PoolingManager.Instance.GetObjectFromPool("PerfectText");
			var position = mainCam.ViewportToWorldPoint(Vector3.one * .5f);
			popupText.transform.position = position;
			var popupTextController = popupText.GetComponent<PopupTextController>();
			popupTextController.textMesh.fontSize = perfectTextSize;
			popupTextController.textMesh.text = "Perfect";
		}
		
		public void ShowComboCounterText(int comboCount)
		{
			if(!ReferenceEquals(_activeComboCounterPopup, null))
			{
				PoolingManager.Instance.ReturnObjectToPool(_activeComboCounterPopup.gameObject, "PopupText");
				_activeComboCounterPopup = null;
			}
			var popupText = PoolingManager.Instance.GetObjectFromPool("PopupText");
			var popupTextController = popupText.GetComponent<PopupTextController>();
			var position = mainCam.ViewportToWorldPoint(Vector3.one * .5f);
			popupText.transform.position = position;
			popupTextController.textMesh.fontSize = comboCounterTextSize;
			popupTextController.textMesh.text = $"{comboCount}X";
			_activeComboCounterPopup = popupTextController;
			popupTextController.Show();
			
		}
	}
}