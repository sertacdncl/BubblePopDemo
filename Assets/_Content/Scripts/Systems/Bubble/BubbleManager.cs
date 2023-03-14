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

		[BoxGroup("References"), SerializeField]
		private BubbleDataPoolHandler bubbleDataPoolHandler;

		#endregion

		#region Variables
		
		#endregion

		public void PrepareStart()
		{
			for (int y = 1; y < GridManager.Instance.GridLength.y-1; y++)
			{
				for (int x = 0; x < GridManager.Instance.GridLength.x; x++)
				{
					//Target cell
					var cellController = GridManager.Instance.CellController[x,y];

					CreateBubbleToCell(cellController);
				}
			}
		}

		private void CreateBubbleToCell(CellController cellController)
		{
			//Getting bubble object from pool
			GameObject bubble = PoolingManager.Instance.GetObjectFromPool("Bubble");
			var bubbleController = bubble.GetComponent<BubbleController>();

			//Setting bubble position and rotation
			bubble.transform.position = Vector3.zero;
			bubble.transform.rotation = Quaternion.identity;

			//Setting bubble parent, reference and name
			cellController.bubbleController = bubbleController;
			bubble.transform.SetParent(cellController.transform, false);
			bubble.name = $"Bubble";

			//Setting bubble data
			var bubbleData = bubbleDataPoolHandler.GetBubbleDataFromPool();
			bubbleController.SetData(bubbleData);
		}

		[Button]
		public void CreateBubbleRow(int rowCoord)
		{
			for (int x = 0; x < GridManager.Instance.GridLength.x; x++)
			{
				var cellController = GridManager.Instance.CellController[x,rowCoord];
				
				CreateBubbleToCell(cellController);
			}
		}
	}
}