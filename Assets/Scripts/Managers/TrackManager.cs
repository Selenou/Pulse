using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TrackManager : MonoBehaviour {

	[Tooltip("Calibrate it for better accuracy (ms)")]
	public int calibrationOffset;
	[Tooltip("The track will start after this time (ms)")]
	public int trackDelay;
	[Tooltip("If you want to start at a specific time (ms)")]
	public int startingTime;
	[Tooltip("The bigger : the slower (ms)")]
	public int bpmToSpeedRatio; 

	private Material redMaterial;
	private Material greenMaterial;
	private Material yellowMaterial;
	private Material blueMaterial;
	private Material orangeMaterial;
	
	public int trackSpeed {get; private set;} // m/s
	private TrackInfo trackInfo;
	private ObjectPooler objectPooler;
	private List<Vector3> bpmBeatPositions;
	private List<Vector3> bpmHalfBeatPositions;
	private int bpmBeatCpt;
	private List<Vector3> notePositions;
	private int noteCpt;
	private int skippedNote;

	void Awake(){
		this.redMaterial = Resources.Load("Materials/Note/RedNote") as Material;
		this.yellowMaterial = Resources.Load("Materials/Note/YellowNote") as Material;
		this.blueMaterial = Resources.Load("Materials/Note/BlueNote") as Material;
		this.orangeMaterial = Resources.Load("Materials/Note/OrangeNote") as Material;
		this.greenMaterial = Resources.Load("Materials/Note/GreenNote") as Material;

		this.bpmBeatPositions = new List<Vector3>();
		this.bpmBeatCpt = 0;
		this.bpmHalfBeatPositions = new List<Vector3>();

		this.notePositions = new List<Vector3>();
		this.noteCpt = 0;
		this.skippedNote = 0;

		this.LoadTrack("Tracks/thePressure");
	}

	private void Start(){
		this.objectPooler = ObjectPooler.Instance;
		this.GenerateTrack();
		this.PlayAudioClip(this.trackInfo.path);
	}

	private void GenerateTrack() {
		this.ResizeNeck();
		this.SpawnNotes();
		this.SpawnBpmBeats();
		this.MoveTrackComponents();
	}

	private void SpawnNotes() {
		foreach(Note note in this.trackInfo.notes) {
			if(note.time >= this.startingTime) {
				float zPos = (this.trackSpeed * note.time) / 1000.0f; // m
				float xPos = (int)note.type - 2; // m
				this.notePositions.Add(new Vector3(xPos, 0.05f, zPos));
				if(this.noteCpt < this.objectPooler.GetSizeOfPoolerFromTag("Note")) {
					this.SpawnNote();
				}
			} else {
				this.skippedNote++;
			}
		}
	}

	public void SpawnNote() {
		// On ne veut pas spawn des notes qui n'existent pas => track terminée
		if(this.noteCpt < this.notePositions.Count) { 
			Note note = this.trackInfo.notes[this.skippedNote + this.noteCpt];
			GameObject newNote = this.objectPooler.SpawnFromPool("Note", this.notePositions[this.noteCpt], Quaternion.identity);
			
			Material mat = this.redMaterial;

			switch (note.type){
				case NoteType.G :
					mat = this.greenMaterial;
					break;
				case NoteType.R :
					mat = this.redMaterial;
					break;
				case NoteType.Y :
					mat = this.yellowMaterial;
					break;
				case NoteType.B :
					mat = this.blueMaterial;
					break;
				case NoteType.O :
					mat = this.orangeMaterial;
					break;
			}

			newNote.gameObject.GetComponent<Renderer>().material = mat;
			
			// We set the note tail size
			float size = (note.duration * this.trackSpeed) / 1000.0f;
			size /= newNote.transform.localScale.z;

			Transform noteTail = newNote.transform.GetChild(0);

			if(size == 0) {
				noteTail.gameObject.SetActive(false);
			} else {
				noteTail.gameObject.SetActive(true);
				noteTail.localScale = new Vector3(0.1f, 0.1f, size);
				noteTail.localPosition = noteTail.localScale / 2.0f;
				noteTail.gameObject.GetComponent<Renderer>().material = mat;
			}
		
			this.noteCpt++;
		}
	}

	private void SpawnBpmBeats() {
		int beatDuration = (int)Math.Round(60000.0f / this.trackInfo.bpm);
		int nbBeat = (int)((this.trackInfo.duration) / beatDuration);
		
		// if we don't start at 0, we need to offset i to display properly all BpmBeats
		int skippedBeat = this.startingTime / beatDuration;

		float halfBeatOffset = ((beatDuration / 2.0f) * this.trackSpeed) / 1000.0f;

		for(int i = skippedBeat; i < nbBeat; i++) {
			float zPos = (i * (beatDuration * this.trackSpeed) / 1000.0f);

			this.bpmBeatPositions.Add(new Vector3(0.0f, 0.01f, zPos));
			this.bpmHalfBeatPositions.Add(new Vector3(0.0f, 0.01f, zPos + halfBeatOffset));

			if(this.bpmBeatCpt < this.objectPooler.GetSizeOfPoolerFromTag("BpmBeat")) {
				this.SpawnBpm();
			}	
		}
	}

	public void SpawnBpm() {
		if(this.bpmBeatCpt < this.bpmBeatPositions.Count) { // Track terminée
			this.objectPooler.SpawnFromPool("BpmBeat", this.bpmBeatPositions[this.bpmBeatCpt], Quaternion.identity);
			this.objectPooler.SpawnFromPool("BpmHalfBeat", this.bpmHalfBeatPositions[this.bpmBeatCpt], Quaternion.identity);
			this.bpmBeatCpt++;
		}
	}

	private void ResizeNeck() {
		//TODO
	}

	private void MoveTrackComponents() {
		GameObject noteContainer = GameObject.Find("AnimatedObject");
		float offset= (calibrationOffset + this.trackDelay - this.startingTime) * this.trackSpeed /1000.0f;
		noteContainer.transform.position = new Vector3(0.0f, 0.0f, offset);
		noteContainer.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -this.trackSpeed);
	}

	private void PlayAudioClip(string path) {
		AudioSource source = this.gameObject.AddComponent<AudioSource>();
		AudioClip clip = Resources.Load<AudioClip>(path);
		source.clip = clip;
		source.time = this.startingTime / 1000.0f;
		source.PlayDelayed(this.trackDelay / 1000.0f);
	}

	private void LoadTrack(string path) {
		TextAsset txt = (TextAsset)Resources.Load(path, typeof(TextAsset));
		this.trackInfo = JsonUtility.FromJson<TrackInfo>(txt.text);
		this.trackSpeed = this.trackInfo.bpm / bpmToSpeedRatio;
	}
}
