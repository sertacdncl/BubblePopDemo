using Pooling;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PopupTextSystem
{
	public class PopupTextManager : Singleton<PopupTextManager>
	{
		#region References

		[BoxGroup("References"), SerializeField]
		private Camera mainCam;

		private PopupTextController _activeComboCounterPopup;

		#endregion

		#region Variables

		[SerializeField] private float popupTextSize = 3f;
		[SerializeField] private float comboCounterTextSize = 15f;
		[SerializeField] private float perfectTextSize = 15f;

		#endregion

		public void  ShowPopupText(int value, Vector3 position)
		{
			var popupText = PoolingManager.Instance.GetObjectFromPool("PopupText");
			var popupTextController = popupText.GetComponent<PopupTextController>();
			popupTextController.textMesh.fontSize = popupTextSize;
			popupTextController.textMesh.text = value.ToString();
			popupText.transform.position = position;
			popupTextController.Show();
		}

		public void ShowPerfectText()
		{
			var popupText = PoolingManager.Instance.GetObjectFromPool("PopupText");
			var position = mainCam.ViewportToWorldPoint(Vector3.one * .5f);
			var popupTextController = popupText.GetComponent<PopupTextController>();
			popupText.transform.position = position;
			popupTextController.textMesh.fontSize = perfectTextSize;
			popupTextController.textMesh.text = "Perfect";
			popupTextController.Show();
		}

		public void ShowComboCounterText(int comboCount)
		{
			if (!ReferenceEquals(_activeComboCounterPopup, null))
			{
				var lastComboCount = int.Parse(_activeComboCounterPopup.textMesh.text.Replace("X", ""));
				if (_activeComboCounterPopup.isPlaying && comboCount == lastComboCount)
					return;

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