using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerCameraFollow))]
public class PlayerInput : MonoBehaviour {

	PlayerCameraFollow player;

	// Use this for initialization
	void Start () {
		player = GetComponent<PlayerCameraFollow> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
