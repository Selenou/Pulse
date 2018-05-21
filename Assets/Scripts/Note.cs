using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour {

	private Rigidbody rb;
	public float speed;

	void Start () {
		this.rb = GetComponent<Rigidbody>();
		this.rb.velocity = new Vector3(0, 0, -speed);
	}
}
