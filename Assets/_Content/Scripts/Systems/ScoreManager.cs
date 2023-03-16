using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
	#region Variables

	public int score;

	#endregion

	public void UpdateScore(int value)
	{
		score += value;
		FrontUIPanelManager.Instance.UpdateInGameScore(ConvertScoreToSuffix(score));
	}

	private string ConvertScoreToSuffix(int value)
	{
		var suffix = "";
		float newValue = value;

		if (newValue >= 1000000000)
		{
			suffix = "B";
			newValue = newValue / 1000000000f;
		}
		else if (newValue >= 1000000)
		{
			suffix = "M";
			newValue = newValue / 1000000f;
		}
		else if (newValue >= 1000)
		{
			suffix = "K";
			newValue = newValue / 1000f;
		}

		if (newValue >= 1000f) //If the value is greater than 1000, one decimal show 
		{
			return $"{newValue:#.#}{suffix}";
		}
		else if (newValue >= 1f)
		{
			return $"{newValue:#.#}{suffix}";
		}
		else
		{
			return $"{newValue:0.##}{suffix}";
		}
	}

	
}
