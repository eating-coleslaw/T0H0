using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class PlayerController : MonoBehaviour {

	public float jumpHeight = 3.5f;
	public float timeToJumpApex = 0.4f;

	public float moveSpeed = 6;
	float accelerationTimeAirborn = 0.2f;
	float accelerationTimeGrounded = 0.1f;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;
	public float wallSlideSpeedMax = 3;

	float gravity;
	float jumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;

	Controller2D controller;

	// Use this for initialization
	void Start () {
		controller = GetComponent<Controller2D> ();

		gravity = (-2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		jumpVelocity = (Mathf.Abs(gravity) * timeToJumpApex);
		print ("Gravity: " + gravity + "   Jump Velocity: " + jumpVelocity);
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		int wallDirX = (controller.collisions.left) ? -1 : 1; // -1=sliding on wall to left, 1=sliding on wall to right

		bool wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;

			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}


		}

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}

		if (Input.GetKeyDown (KeyCode.Space)) { //remove for walljumping && controller.collisions.below) {
			if (wallSliding) {
				//wall climb jump
				if (wallDirX == input.x) {
					velocity.x = -wallDirX * wallJumpClimb.x;
					velocity.y = wallJumpClimb.y;
				}
				//jump off wall
				else if (input.x == 0) {
					velocity.x = -wallDirX * wallJumpOff.x;
					velocity.x = wallJumpOff.y;
				}
				//leap off wall
				else {
					velocity.x = -wallDirX * wallLeap.x;
					velocity.y = wallLeap.y;
				}
			}
			//normal ground jump
			if (controller.collisions.below) {
			velocity.y = jumpVelocity;
			}
		}

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborn);
		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
	}
}
