using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FrontUIPanelManager : Singleton<FrontUIPanelManager>
{
	#region References

	[BoxGroup("References"), SerializeField]
	private Image panelImage;

	[BoxGroup("References"), SerializeField]
	private TextMeshProUGUI endPanelScoreText;
	
	[BoxGroup("References"), SerializeField]
	private TextMeshProUGUI inGameScoreText;

	#endregion
	
	public void GameOver()
	{
		panelImage.transform.localScale = Vector3.zero;
		panelImage.gameObject.SetActive(true);
		panelImage.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
		endPanelScoreText.text = $"{ScoreManager.Instance.score}";
	}

	public void OnClick_TryAgain()
	{
		SceneManager.LoadScene(0);
	}

	public void UpdateInGameScore(string score)
	{
		inGameScoreText.text = $"{score}";
	}
}