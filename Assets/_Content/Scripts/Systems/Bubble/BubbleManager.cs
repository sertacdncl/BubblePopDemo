using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BubbleSystem
{
	public class BubbleManager : Singleton<BubbleManager>
	{
		#region References

		[BoxGroup("References"), SerializeField, TableList]
		private List<BubbleData> bubbleDataList;

		#endregion

		#region Variables

		[BoxGroup("Variables"), SerializeField]
		private int startRow = 4;

		#endregion

		public void PrepareStart()
		{
			for (int y = 0; y < y; y++)
			{
			}
		}
	}
}