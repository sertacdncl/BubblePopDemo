using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Helper
{
	public static class Helpers
	{
		//Option enum can be extended
		public enum Option
		{
			Two,
			Four,
			Eight,
			Sixteen,
			ThirtyTwo,
			SixtyFour,
			OneHundredTwentyEight,
			TwoHundredFiftySix,
			FiveHundredTwelve,
			OneThousandTwentyFour,
			TwoThousandFortyEight
		}

		[Serializable]
		public class OptionData
		{
			public float Weight;
			public Option MyOption;

			public OptionData(Option myOption, float weight)
			{
				MyOption = myOption;
				Weight = weight;
			}
		}

		public static Option GetValue(List<OptionData> listOptionsData)
		{
			float max = 0;
			foreach (var item in listOptionsData)
				max += item.Weight;

			var randomValue = Random.Range(0f, max);

			float count = 0;
			foreach (var optionsData in listOptionsData)
			{
				count += optionsData.Weight;
				if (randomValue <= count)
					return optionsData.MyOption;
			}

			return Option.Two;
		}
	}
}