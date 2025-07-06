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
		
		public List<GameObject> PoolObjects => new List<GameObject>(pool);

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
				PoolObjects.Add(poolObject);
			}
		}

		public GameObject GetPoolObject()
		{
			if (pool.Count == 0)
			{
#if UNITY_EDITOR
				Debug.LogWarning($"{poolName} pool empty. Adding a new object to the pool.");
#endif

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
			poolObject.transform.SetParent(transform);
			pool.Enqueue(poolObject);
		}
	}
}