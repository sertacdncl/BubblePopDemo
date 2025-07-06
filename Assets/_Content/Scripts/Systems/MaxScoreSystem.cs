using UnityEngine;

public static class MaxScoreSystem
{
	private const string MaxScoreKey = "MaxScore";

	public static int GetMaxScore()
	{
		if (!PlayerPrefs.HasKey(MaxScoreKey))
			PlayerPrefs.SetInt(MaxScoreKey, 0);

		return PlayerPrefs.GetInt(MaxScoreKey, 0);
	}

	public static string GetMaxScoreText()
	{
		return ConvertScoreToSuffix(GetMaxScore());
	}

	public static void SetMaxScore(int score)
	{
		if (score <= GetMaxScore()) return;

		PlayerPrefs.SetInt(MaxScoreKey, score);
		PlayerPrefs.Save();
	}
	
	private static string ConvertScoreToSuffix(int value)
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