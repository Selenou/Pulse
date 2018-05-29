using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

	[Serializable]
	public class Pool {
		public string tag;
		public GameObject prefab;
		public int size;
		public Transform parent;
	}
	
	public static ObjectPooler Instance;
	public List<Pool> pools;
	public Dictionary<string, List<GameObject>> poolDictionary;

	private void Awake() {
		if(Instance == null) {
			Instance = this;
		} else if(Instance != this){
			Destroy(this.gameObject);
		}

		this.poolDictionary = new Dictionary<string, List<GameObject>>();

		foreach (Pool pool in this.pools) {
			List<GameObject> objectPool = new List<GameObject>();

			for(int i =0; i < pool.size; i++) {
				GameObject obj = Instantiate(pool.prefab, pool.parent);
				obj.SetActive(false);
				objectPool.Add(obj);
			}

			this.poolDictionary.Add(pool.tag, objectPool);
		}
	}

	public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation) {
		if(!this.poolDictionary.ContainsKey(tag)) {
			Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
			return null;
		}

		foreach(GameObject go in this.poolDictionary[tag]){
			if(!go.activeInHierarchy) {
				go.SetActive(true);
				go.transform.localPosition = position;
				go.transform.localRotation = rotation;
				return go;
			}
		}

		Debug.LogWarning("No GO of type " + tag + " are available for now, increase pool's size ?");
		
		return null;
	}

	public int GetSizeOfPoolerFromTag(string tag) {
		if(!this.poolDictionary.ContainsKey(tag)) {
			return 0;
		}
		return this.poolDictionary[tag].Count;
	}
}
