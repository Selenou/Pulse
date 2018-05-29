using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BpmGarbageCollector : MonoBehaviour {

	private TrackManager trackManager;
	void Start () {
		this.trackManager = GameObject.Find("Track").GetComponent<TrackManager>();
	}

	void OnTriggerEnter(Collider other){
		if(other.tag.Equals("BpmBeat")) { 
			other.gameObject.SetActive(false);
		} else if(other.tag.Equals("BpmHalfBeat")) {
			other.gameObject.SetActive(false);
			this.trackManager.SpawnBpm(); // On pop des nouveau bpm que sur le demi-temps 
		}
	}
}
