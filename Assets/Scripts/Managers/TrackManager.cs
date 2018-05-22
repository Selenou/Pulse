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
	private int trackSpeed; // m/s
	private GameObject greenNotePrefab;
	private GameObject redNotePrefab;
	private GameObject yellowNotePrefab;
	private GameObject blueNotePrefab;
	private GameObject orangeNotePrefab;
	private TrackInfo trackInfo;

	void Awake(){
		this.greenNotePrefab = Resources.Load("Prefabs/GreenNote") as GameObject;
		this.redNotePrefab = Resources.Load("Prefabs/RedNote") as GameObject;
		this.yellowNotePrefab = Resources.Load("Prefabs/YellowNote") as GameObject;
		this.blueNotePrefab = Resources.Load("Prefabs/BlueNote") as GameObject;
		this.orangeNotePrefab = Resources.Load("Prefabs/OrangeNote") as GameObject;
	}
	
	void Start () {
		this.LoadTrack("Tracks/thePressure");
		this.GenerateTrack();
		this.PlayAudioClip(this.trackInfo.path);
	}

	private void GenerateTrack() {
		this.trackSpeed = this.trackInfo.bpm / bpmToSpeedRatio;
		foreach(Note note in this.trackInfo.notes) {
			this.SpawnNote(note);
		}
	}

	private void SpawnNote(Note note) {
		float finalNoteTime = (note.time + calibrationOffset + this.trackDelay - this.startingTime) / 1000.0f;
		float zPos = this.trackSpeed * finalNoteTime;
		float xPos = 0.0f;
		GameObject prefab = null;
		
		switch (note.type){
			case NoteType.G :
				xPos = -0.5f;
				prefab = this.greenNotePrefab;
				break;
			case NoteType.R :
				xPos = 0.5f;
				prefab = this.redNotePrefab;
				break;
			case NoteType.Y :
				xPos = 1.5f;
				prefab = this.yellowNotePrefab;
				break;
			case NoteType.B :
				xPos = 2.5f;
				prefab = this.blueNotePrefab;
				break;
			case NoteType.O :
				xPos = 3.5f;
				prefab = this.orangeNotePrefab;
				break;
		}

		GameObject newNote = Instantiate(prefab, new Vector3(xPos, 0.05f, zPos), Quaternion.identity, GameObject.Find("Notes").transform);
		newNote.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -this.trackSpeed);
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
	}
}
