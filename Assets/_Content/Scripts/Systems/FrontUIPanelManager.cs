using System.Text;
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

	[BoxGroup("References"), SerializeField]
	private TextMeshProUGUI maxScoreText;

	private const string MaxScoreText = "Max Score: ";

	#endregion

	public void GameOver()
	{
		panelImage.transform.localScale = Vector3.zero;
		panelImage.gameObject.SetActive(true);
		panelImage.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
		endPanelScoreText.text = $"{ScoreManager.Instance.Score}";

		maxScoreText.text = new StringBuilder().Append(MaxScoreText).Append(MaxScoreSystem.GetMaxScoreText()).ToString();
	}

	public void OnClick_TryAgain()
	{
		SceneManager.LoadScene("Game");
	}

	public void UpdateInGameScore(string score)
	{
		inGameScoreText.text = $"{score}";
	}
}