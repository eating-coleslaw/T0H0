using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (PlayerController))]
public class AnimController2D : MonoBehaviour {

	Controller2D controller2D;
	PlayerController playerController;

	SpriteAnim m_anim = null;
	public AnimationClip m_idle = null;
	public AnimationClip m_run = null;
	public AnimationClip m_jump = null;
	public AnimationClip m_land = null;
	public AnimationClip m_wallStick = null;
	public AnimationClip m_wallLeap = null;
	public AnimationClip m_wallSlide = null;
	public AnimationClip m_attack = null;
	public AnimationClip m_airB = null;
	public AnimationClip m_airF = null;

	AnimationClip m_wasPlaying = null;

	Queue m_clip_queue;


	// Use this for initialization
	void Start () {
		controller2D = GetComponent<Controller2D> ();
		playerController = GetComponent<PlayerController> ();

		m_anim = GetComponent<SpriteAnim> ();
		m_anim.Play(m_idle);

		m_clip_queue = new Queue();

	}
	
	//Called at beginning of each PlayerController update to set the animation
	//that was just playing
	public void SetWasPlayingClip (AnimationClip m_clip) {
		if (m_clip != null) {
			m_wasPlaying = m_clip;
		}
	}

	//Start or continue the idle animation
	public void HandleIdle () {
		PlayIfNotWasPlaying(m_idle);
	}

	public void HandleNormalJump() {
		PlayIfNotWasPlaying(m_jump);
	}

	// Play animation for when playing is moving forwards or backwards through
	// the air
	public void HandleAirHangTime() {
		if (Mathf.Sign(playerController.velocity.x) < 0) {
			m_anim.Play(m_airB);
		} else {
			m_anim.Play(m_airF);
		}
	}

	public void HandleWallOff() {
		PlayIfNotWasPlaying(m_wallLeap);
		m_clip_queue.Clear();
	}

	// Transition to wall sliding, or continue wall-sliding. In either case, handle
	// flipping the animation sprite direction.
	public void HandleWallOn(bool wasWallSliding, float wallDirX) {
		if (wasWallSliding) {
			PlayIfNotWasPlaying(m_wallSlide);
		} else {
			PlayIfNotWasPlaying(m_wallStick);
			m_clip_queue.Clear();
			m_clip_queue.Enqueue(m_wallSlide);
		}
		//Flip if sliding on wall to right
		if (wallDirX == 1) {
			FlipDirection(wallDirX);
		}
	}

	// Flip the horizontal (left-right) direction of the object
	void FlipDirection(float direction) {
		if ( Mathf.Sign(transform.localScale.x) != Mathf.Sign(direction)) {
			transform.localScale = new Vector3( Mathf.Sign(direction), 1, 1);
		}
	}

	//Play an animation if it wasn't the animation playing before player input
	void PlayIfNotWasPlaying () {
		m_wasPlaying = m_anim.GetCurrentAnimation();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
