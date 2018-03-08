using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

[RequireComponent (typeof (Controller2D))]
//[RequireComponent (typeof (AnimController2D))]
public class PlayerController : MonoBehaviour {

	public float maxJumpHeight = 3.5f;
	public float minJumpHeight = 1f;
	public float maxDoubleJumpHeight = 2.75f;
	public float timeToJumpApex = 0.4f;
	public float timeFromJumpApexToGround = 0.2f;
	public float timeToDoubleJumpApex = 0.3f;

	public float minRunSpeed = 2;
	public float moveSpeed = 6;
	public float accelerationTimeAirborn = 0.2f;
	public float accelerationTimeGrounded = 0.1f;
	public float doubleJumpXModifier = 1.2f;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = 0.25f;
	float timeToWallUnstick;

	float gravity;
	float fallGravity;
	float doubleJumpGravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	float maxDoubleJumpVelocity;
	float minDoubleJumpVelocity;
	[HideInInspector]
	public Vector3 velocity;
	float velocityXSmoothing;

	Controller2D controller;
	Vector2 directionalInput;
	bool wallSliding;
	int wallDirX;
	bool m_movingForwards;
	bool m_allowJump;
	bool m_allowDoubleJump;

	private Animator animator;
	private SpriteRenderer spriteRenderer;

	//SpriteAnim m_anim = null;
	//AnimController2D m_animController = null;
	//AnimationClip m_nowPlaying = null;

	// Use this for initialization
	void Start () {
		controller = GetComponent<Controller2D> ();
		//m_anim = GetComponent<SpriteAnim> ();
		//m_animController = GetComponent<AnimController2D> ();

		animator = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();

		
		// Jumping Gravity
		gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		doubleJumpGravity = (-2 * maxJumpHeight);
		maxJumpVelocity = (Mathf.Abs(gravity) * timeToJumpApex);
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
		print ("Gravity: " + gravity + "   Jump Velocity: " + maxJumpVelocity);

		// Double Jump Gravity & Velocities
		doubleJumpGravity = (-2 * maxDoubleJumpHeight);
		maxDoubleJumpVelocity = (Mathf.Abs(doubleJumpGravity) * timeToDoubleJumpApex);
		minDoubleJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(doubleJumpGravity) * minJumpHeight);

		// Falling Gravity
		fallGravity = (-2 * maxJumpHeight) / Mathf.Pow(timeFromJumpApexToGround, 2);
	}

	// Update is called once per frame
	void Update () {
		//m_animController.SetWasPlayingClip();

		CalculateVelocity();
		HandleWallSliding();

		controller.Move(velocity * Time.deltaTime, directionalInput);

		//call here to make sure we're incorporating platform influence on player
		if (controller.collisions.above || controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				velocity.y += controller.collisions.slopeNormal.y * -fallGravity * Time.deltaTime;
			} else {
				velocity.y = 0;
			}
		}

		// Handle Sprite Flipping
		if (controller.collisions.below) {
			bool flipSprite = (spriteRenderer.flipX ? (velocity.x > 0.01f) : (velocity.x < -0.01f));
			if (flipSprite) {
				spriteRenderer.flipX = !spriteRenderer.flipX;
			}
		} else if (wallSliding) {
			bool flipSprite = spriteRenderer.flipX ? controller.collisions.left : controller.collisions.right;
			if (flipSprite) {
				spriteRenderer.flipX = !spriteRenderer.flipX;
			}
		}

		m_movingForwards = (Mathf.Sign(velocity.x) == (spriteRenderer.flipX ? -1 : 1) );
		bool isWalking = (Mathf.Abs(velocity.x) < minRunSpeed);

		animator.SetBool("IsWalking",isWalking);
		animator.SetBool("MovingForwards",m_movingForwards);
		animator.SetBool("Grounded",controller.collisions.below);
		animator.SetFloat("VelocityX",velocity.x); //Mathf.Abs(velocity.x) / moveSpeed);
		animator.SetFloat("MoveX",Mathf.Abs(velocity.x) / moveSpeed);
		animator.SetFloat("VelocityY",velocity.y);
		animator.SetFloat("MoveY",Mathf.Abs(velocity.y) * Time.deltaTime);
		animator.SetBool("WallSliding",wallSliding);


		/* if (wallSliding) {
			return;
		}
		//Handle remaining animation cases
		else if (IsStandingStill()) {
			m_animController.HandleIdle();
		}
		else if (IsLanding()) {
			m_animController.HandleLanding();
		}
		else if (IsInHangTime()) {
			m_animController.HandleAirHangTime();
		}
		else if (IsRunning()) {
			m_animController.HandleRunning(Mathf.Sign(directionalInput.x));
		}
		else {
			m_animController.HandleIdle();
		} */
	}

	public void SetDirectionalInput (Vector2 input) {
		directionalInput = input;
	}

	public void OnJumpInputDown() {
		if (wallSliding) {
				m_allowDoubleJump = true;
				//wall climb jump
				if (wallDirX == directionalInput.x) {
					velocity.x = -wallDirX * wallJumpClimb.x;
					velocity.y = wallJumpClimb.y;
				}
				//jump/fall off wall
				else if (directionalInput.x == 0) {
					velocity.x = -wallDirX * wallJumpOff.x;
					velocity.x = wallJumpOff.y;
				}
				//leap off wall
				else {
					velocity.x = -wallDirX * wallLeap.x;
					velocity.y = wallLeap.y;
				}
				//m_animController.HandleWallOff();
			}
			
		//normal ground jump
		else if (controller.collisions.below) {
			m_allowDoubleJump = true;
			if (controller.collisions.slidingDownMaxSlope) {
				if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)) { //not jumping against max slope
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			} else {
				velocity.y = maxJumpVelocity;
			}
			//m_animController.HandleNormalJump();
		} else if (m_allowDoubleJump) {
			velocity.y = maxDoubleJumpVelocity;
			m_allowDoubleJump = false;
		}
	}

	public void OnJumpInputUp() {
		// If we can double jump, then we're not double jumping!
		if (m_allowDoubleJump) {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}
		// Otherwise, we are double jumping
		else {
			if (velocity.y > minDoubleJumpVelocity) {
				velocity.y = minDoubleJumpVelocity;
			}
		}
	}

	void CalculateVelocity() {
		float targetVelocityX = directionalInput.x * moveSpeed;
		
		//Double jump has more emphasis on lateral movement
		if (m_allowDoubleJump == false) {
			targetVelocityX *= doubleJumpXModifier;
		}
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborn);
		
		// Different jump velocities, but same falling velocity
		if (velocity.y >= 0) {
			velocity.y += (m_allowDoubleJump) ? (gravity * Time.deltaTime) : (doubleJumpGravity * Time.deltaTime);
			//velocity.y += ((velocity.y >= 0) ? (gravity * Time.deltaTime) : (fallGravity * Time.deltaTime));
		} else {
			velocity.y += fallGravity * Time.deltaTime;
			}
	}


	bool IsInHangTime() {
		if (!controller.collisions.below && !controller.collisions.above && !controller.collisions.left && !controller.collisions.right) {
			return true;
		} else {
			return false;
		}
	}

	bool IsStandingStill() {
		if ( (Mathf.Abs(velocity.x) < 0.1) && (Mathf.Abs(velocity.y) < 0.1) && controller.collisions.below) {
			return true;
		} else {
			return false;
		}
	}

	bool IsRunning() {
		if ( controller.collisions.below && (Mathf.Abs(velocity.y) < 0.1) && (Mathf.Abs(velocity.x) > 0.1|| directionalInput.x != 0) ) {
			return true;
		} else {
			return false;
		}
	}

	bool IsLanding() {
		if ( (controller.collisions.below && velocity.y < 0) /*|| m_animController.IsStillLanding()*/ ) {
			return true;
		} else {
			return false;
		}
	}

	void HandleWallSliding() {
		wallDirX = (controller.collisions.left) ? -1 : 1; // -1=sliding on wall to left, 1=sliding on wall to right
		bool wasWallSliding = wallSliding;
		wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;

			
			//m_animController.HandleWallOn(wasWallSliding, wallDirX);

			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {

				velocityXSmoothing = 0;
				velocity.x = 0;

				if (directionalInput.x != wallDirX && directionalInput.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				}
				else {
					timeToWallUnstick = wallStickTime;
				}
			}
			else {
				timeToWallUnstick = wallStickTime;
			}
		}
	}
}
