using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour {

	public GameObject note;

	void Start () {
		Instantiate(note, new Vector3(0f, 0.05f, 23.7f), Quaternion.identity, GameObject.Find("Notes").transform);
	}
}
