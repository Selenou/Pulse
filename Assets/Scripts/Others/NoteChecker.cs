using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteChecker : MonoBehaviour {

	public int lenience; // ms
	public KeyCode keycode;
	public Material trackMaterial;
	private ScoreManager scoreManager;
	private TrackManager trackManager;
	private Material baseMaterial;
	private int longNotePlayingCpt;
	private Queue<GameObject> noteQueue;

	void Start() {
		this.scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
		this.trackManager = GameObject.Find("Track").GetComponent<TrackManager>();
		this.baseMaterial = Resources.Load("Materials/Track/NoteTrack/Transparent") as Material;

		// Set Hitnox with lenience
		this.ConfigureLenience();

		this.longNotePlayingCpt = 0;
		this.noteQueue = new Queue<GameObject>();
	}

	void Update () {
		if(Input.GetKeyDown(this.keycode)) {
			this.ChangeTrackMaterial(this.trackMaterial);

			if(this.noteQueue.Count > 0) {
				GameObject note = this.noteQueue.Peek();

				//float delay = (note.transform.position.z * 1000.0f) / this.trackManager.trackSpeed;
				//Debug.Log("delay : " + delay + " " + Time.time);

				this.scoreManager.UpdateScore();

				if(note.transform.GetChild(0).gameObject.activeSelf) { // Si c'est une note longue
					this.longNotePlayingCpt++;
					this.scoreManager.StartLongNoteScoring(this.gameObject.name);
				} else {
					this.RecycleNote();
				}
			} else { // Si le joueur a mal cliqué
				this.scoreManager.ResetScore();
			}
		} else if(Input.GetKeyUp(this.keycode)) {
			this.ChangeTrackMaterial(this.baseMaterial);

			if(this.longNotePlayingCpt > 0) { // Si c'est une note longue
				this.longNotePlayingCpt--;
				this.scoreManager.StopLongNoteScoring(this.gameObject.name);
				this.RecycleNote(); // important ici lorsque qu'on a 2 notes longues a suivre
			}
		}
	}

	private void ConfigureLenience() {
		float sizeZ = (this.lenience / 1000.0f) * this.trackManager.trackSpeed;
		this.GetComponent<BoxCollider>().size = new Vector3(1, 1, sizeZ);
	}

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag.Equals("Note")) {
			this.noteQueue.Enqueue(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other) {
		if(other.gameObject.tag.Equals("Note") && this.longNotePlayingCpt == 0) { // If it's a note without a duration : player just missed it
			this.scoreManager.ResetScore();
			this.RecycleNote();
		} else if(other.gameObject.tag.Equals("NoteTail")) { // If it's a note with a duration
			if(this.longNotePlayingCpt > 0) {
				this.longNotePlayingCpt--;
				this.scoreManager.StopLongNoteScoring(this.gameObject.name);
			}
			this.RecycleNote();
		}
	}

	private void RecycleNote() {
		GameObject note = this.noteQueue.Dequeue();
		note.SetActive(false);
		this.trackManager.SpawnNote();
	}

	private void ChangeTrackMaterial(Material mat) {
		this.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = mat;
	}
}
