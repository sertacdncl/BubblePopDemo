using System.Text;
using TMPro;
using UnityEngine;

public class InitSceneUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionText;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private const string MaxScoreText = "Max Score: ";
    
    private void Start()
	{
		versionText.text = Application.version;
		
		if (scoreText != null)
			scoreText.text = new StringBuilder().Append(MaxScoreText).Append(MaxScoreSystem.GetMaxScoreText()).ToString();
		else
			Debug.LogWarning("Score Text is not assigned in InitSceneUI.");
	}
    
    
}
