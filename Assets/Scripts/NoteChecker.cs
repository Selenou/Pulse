using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteChecker : MonoBehaviour {

	public KeyCode keycode;
	public bool isActive;
	public ScoreManager scoreManager;
	private GameObject note;
	private Material material;
	private Color activeColor;
	private Color inactiveColor;

	void Start () {
		this.material = GetComponent<SpriteRenderer>().material;
		this.activeColor = this.material.color;
		this.inactiveColor = this.material.GetColor("_EmissionColor");
	}

	void Update () {
		if(Input.GetKeyDown(this.keycode)) {
			this.material.SetColor("_EmissionColor", this.activeColor);
			
			if(this.isActive) {
				Debug.Log("OK " + this.note.transform.position.z * 6 / 170 + " " + + this.note.transform.position.z);
				this.isActive = false;
				Destroy(this.note);
				this.scoreManager.UpdateScore();
			} else {
				this.scoreManager.ResetScore();
				Debug.Log("MISS " + this.note.transform.position.z * 6 / 170 + " " + + this.note.transform.position.z);
			}
		}

		if(Input.GetKeyUp(this.keycode)) {
			this.material.SetColor("_EmissionColor", this.inactiveColor);
		}
	}

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Note") {
			this.isActive = true;
			this.note = other.gameObject;
		}
	}

	private void OnTriggerExit(Collider other) {
		this.isActive = false;
		Destroy(this.note);
		this.scoreManager.ResetScore();
	}
}
