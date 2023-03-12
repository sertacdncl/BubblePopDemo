using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BubbleManager : Singleton<BubbleManager>
{
	[SerializeField, TableList] 
	private List<BubbleData> bubbleDataList;
	
	
}