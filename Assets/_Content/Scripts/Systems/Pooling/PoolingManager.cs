using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Pooling
{
	public class PoolingManager : Singleton<PoolingManager>
	{
		#region References

		[BoxGroup("References"), SerializeField]
		private List<ObjectPool> pools;

		#endregion

		private void Awake()
		{
			if(pools.Count == 0)
				Debug.LogError("Pool list is empty!");
		}

		public GameObject GetObjectFromPool(string poolName)
		{
			var pool = pools.Find(x => string.Equals(x.poolName, poolName, StringComparison.CurrentCultureIgnoreCase));
			return pool.GetPoolObject();
		}

		public void ReturnObjectToPool(GameObject poolObject, string poolName)
		{
			var pool = pools.Find(x => x.poolName == poolName);
			pool.ReturnObjectToPool(poolObject);
		}
	}
}