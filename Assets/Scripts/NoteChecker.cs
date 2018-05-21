using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteChecker : MonoBehaviour {

	public KeyCode keycode;
	public bool isActive;
	private GameObject note;
	private SpriteRenderer renderer;

	void Start () {
		this.renderer = GetComponent<SpriteRenderer>();
		this.renderer.color = Color.yellow;
	}

	void Update () {
		if(Input.GetKeyDown(this.keycode)) {
			this.renderer.color = Color.green;

			if(this.isActive) {
				Destroy(this.note);
			}
		}

		if(Input.GetKeyUp(this.keycode)) {
			this.renderer.color = Color.yellow;
		}
	}

	private void OnTriggerEnter(Collider other) {
		this.isActive = true;

		if(other.gameObject.tag == "Note") {
			this.note = other.gameObject;
		}
	}

	private void OnTriggerExit(Collider other) {
		this.isActive = false;
	}
}
