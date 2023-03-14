using System;
using System.Collections.Generic;
using BubbleSystem;
using Helper;
using Sirenix.OdinInspector;
using UnityEngine;

public class BubbleDataPoolHandler : MonoBehaviour
{
	#region References

	[BoxGroup("Pool Settings"), SerializeField]
	private List<Helpers.OptionData> listOptionsData;

	[BoxGroup("References"), SerializeField, TableList]
	private List<BubbleData> bubbleDataList;

	private List<BubbleData> listBubbleData;

	#endregion

	#region Variables

	[BoxGroup("Variables"),SerializeField] private int initialPoolSize = 100;

	#endregion

	private void Start()
	{
		CreateBubbleDataPool();
	}

	public BubbleData GetBubbleDataFromPool()
	{
		if(listBubbleData.Count == 0)
			CreateBubbleDataPool();
		
		var bubbleData = listBubbleData[0];
		listBubbleData.RemoveAt(0);
		return bubbleData;
	}

	private void CreateBubbleDataPool()
	{
		listBubbleData = new List<BubbleData>();
		for (int i = 0; i < initialPoolSize; i++)
		{
			var option = Helpers.GetValue(listOptionsData);
			var bubbleData = GetBubbleDataFromOption(option);
			listBubbleData.Add(bubbleData);
		}
	}

	private BubbleData GetBubbleDataFromOption(Helpers.Option option)
	{
		var bubbleData = option switch
		{
			Helpers.Option.Two => bubbleDataList[0],
			Helpers.Option.Four => bubbleDataList[1],
			Helpers.Option.Eight => bubbleDataList[2],
			Helpers.Option.Sixteen => bubbleDataList[3],
			Helpers.Option.ThirtyTwo => bubbleDataList[4],
			Helpers.Option.SixtyFour => bubbleDataList[5],
			Helpers.Option.OneHundredTwentyEight => bubbleDataList[6],
			Helpers.Option.TwoHundredFiftySix => bubbleDataList[7],
			Helpers.Option.FiveHundredTwelve => bubbleDataList[8],
			Helpers.Option.OneThousandTwentyFour => bubbleDataList[9],
			Helpers.Option.TwoThousandFortyEight => bubbleDataList[10],
			_ => bubbleDataList[0]
		};

		return bubbleData;
	}
}