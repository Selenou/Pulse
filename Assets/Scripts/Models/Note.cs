using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoteType {G,R,Y,B,O};

[Serializable]
public class Note  {
	public int time;
	public int duration;
	public NoteType type;
}
