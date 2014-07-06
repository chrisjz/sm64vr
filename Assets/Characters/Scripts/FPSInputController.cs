using UnityEngine;
using System.Collections;

// Require a character controller to be attached to the same game object
[RequireComponent(typeof(CharacterMotor))]
[AddComponentMenu("Character/FPS Input Controller")]

public class FPSInputController : MonoBehaviour {
	public GameObject ovrCamera;
	public AudioClip[] initialJumpAudioClips;

	private CharacterMotor motor;
	private float defaultMaxForwardSpeed;
	private float defaultMaxBackwardsSpeed;
	private bool inputEnabled;					// If input is enabled/disabled
	
	// Use this for initialization
	void  Awake (){
		motor = GetComponent<CharacterMotor>();
		defaultMaxForwardSpeed = motor.movement.maxForwardSpeed;
		defaultMaxForwardSpeed = motor.movement.maxBackwardsSpeed;
	}

	void Start() {
		IgnorePlayerColliders ();
		inputEnabled = true;
	}
	
	// Update is called once per frame
	void  Update (){
		if (!inputEnabled) {
			return;
		}

		// Get the input vector from keyboard or analog stick
		Vector3 directionVector= new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		// Get the input vector from hydra
		SixenseInput.Controller hydraLeftController = SixenseInput.GetController (SixenseHands.LEFT);
		SixenseInput.Controller hydraRightController = SixenseInput.GetController (SixenseHands.RIGHT);

		if (hydraLeftController != null) {
			directionVector= new Vector3(hydraLeftController.JoystickX, 0, hydraLeftController.JoystickY);
		}

		if (hydraRightController != null) {
			motor.inputJump = hydraRightController.GetButton (SixenseButtons.BUMPER);
		} else {
			motor.inputJump = Input.GetButton ("Jump");
		}

		// Play jumping audio clips
		if (initialJumpAudioClips.Length > 0 && motor.inputJump && motor.grounded && !audio.isPlaying) {
			audio.clip = initialJumpAudioClips[Random.Range(0, initialJumpAudioClips.Length)];
			audio.Play();
		}
		
		if (directionVector != Vector3.zero) {
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			float directionLength= directionVector.magnitude;
			directionVector = directionVector / directionLength;
			
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1, directionLength);
			
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
			
			// Multiply the normalized direction vector by the modified length
			directionVector = directionVector * directionLength;
		}
		
		// Apply the direction to the CharacterMotor
		motor.inputMoveDirection = ovrCamera.transform.rotation * directionVector;

		UpdateAnimations (directionVector);
	}

	void UpdateAnimations(Vector3 directionVector) {
		if (animation && directionVector.z != 0) {
			animation["Walk"].speed = Mathf.Abs(directionVector.z);
			animation.CrossFade("Walk");
		}
	}

	public void SetInputEnabled (bool status) {
		inputEnabled = status;
	}

	// Prevent colliders on player from colliding with each other i.e. hand colliders with body collider
	void IgnorePlayerColliders () {
		Collider[] cols = GetComponentsInChildren<Collider>();

		foreach (Collider col in cols) {
			if (col != collider) {
				Physics.IgnoreCollision(col, collider);
			}
		}
	}
	
	public float getDefaultMaxForwardSpeed () {
		return defaultMaxForwardSpeed;
	}
	
	public float getDefaultMaxBackwardsSpeed () {
		return defaultMaxForwardSpeed;
	}
}