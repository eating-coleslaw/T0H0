using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerCameraFollow))]
public class PlayerInput : MonoBehaviour {

	PlayerController player;

	// Use this for initialization
	void Start () {
		player = GetComponent<PlayerCameraFollow> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		player.SetDirectionalInput(directionalInput);
	}
}
