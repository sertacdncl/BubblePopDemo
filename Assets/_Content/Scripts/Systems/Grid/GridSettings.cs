using System;
using UnityEngine;

namespace GridSystem
{
	[Serializable]
	public class GridSettings
	{
		public GameObject cellPrefab;
		public Vector2 distance;
		public Vector2Int rowColumnSize;
	}
}