using System.Collections.Generic;
using GridSystem;
using Pooling;
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
		
		#endregion

		public void PrepareStart()
		{
			for (int y = 1; y < GridManager.Instance.GridLength.y-1; y++)
			{
				for (int x = 0; x < GridManager.Instance.GridLength.x; x++)
				{
					GameObject bubble = PoolingManager.Instance.GetObjectFromPool("Bubble");
					bubble.transform.position = Vector3.zero;
					bubble.transform.rotation = Quaternion.identity;
					var cellController = GridManager.Instance.CellController[x,y];
					cellController.bubbleController = bubble.GetComponent<BubbleController>();
					bubble.transform.SetParent(cellController.transform, false);
					bubble.name = $"Bubble";
				}
			}
		}
		
		[Button]
		public void CreateBubbleRow(int rowCoord)
		{
			for (int x = 0; x < GridManager.Instance.GridLength.x; x++)
			{
				GameObject bubble = PoolingManager.Instance.GetObjectFromPool("Bubble");
				bubble.transform.position = Vector3.zero;
				bubble.transform.rotation = Quaternion.identity;
				var cellController = GridManager.Instance.CellController[x,rowCoord];
				cellController.bubbleController = bubble.GetComponent<BubbleController>();
				bubble.transform.SetParent(cellController.transform, false);
				bubble.name = $"Bubble";
			}
		}
	}
}