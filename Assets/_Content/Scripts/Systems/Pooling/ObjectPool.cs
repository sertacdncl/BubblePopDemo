using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pooling
{
	public class ObjectPool : MonoBehaviour
	{
		#region References

		[BoxGroup("References")] [SerializeField]
		private GameObject poolObjectPrefab;

		private Queue<GameObject> pool = new();

		#endregion

		#region Variables

		[BoxGroup("Variables")] public string poolName;

		[BoxGroup("Variables")] [SerializeField]
		private int initialPoolSize = 50;

		#endregion


		private void Awake()
		{
			for (int i = 0; i < initialPoolSize; i++)
			{
				GameObject poolObject = Instantiate(poolObjectPrefab, transform);
				poolObject.SetActive(false);
				pool.Enqueue(poolObject);
			}
		}

		public GameObject GetPoolObject()
		{
			if (pool.Count == 0)
			{
				Debug.LogWarning($"{poolName} pool empty. Adding a new object to the pool.");
				GameObject poolObject = Instantiate(poolObjectPrefab);
				poolObject.SetActive(false);
				pool.Enqueue(poolObject);
			}

			GameObject objectFromPool = pool.Dequeue();
			objectFromPool.SetActive(true);
			return objectFromPool;
		}

		public void ReturnObjectToPool(GameObject poolObject)
		{
			poolObject.SetActive(false);
			pool.Enqueue(poolObject);
		}
	}
}