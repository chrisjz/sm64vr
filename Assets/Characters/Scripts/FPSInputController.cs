using UnityEngine;
using System.Collections;

// Require a character controller to be attached to the same game object
[RequireComponent(typeof(CharacterMotor))]
[AddComponentMenu("Character/FPS Input Controller")]

public class FPSInputController : MonoBehaviour {
	private CharacterMotor motor;

	public GameObject ovrCamera;
	
	// Use this for initialization
	void  Awake (){
		motor = GetComponent<CharacterMotor>();
	}
	
	// Update is called once per frame
	void  Update (){
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
	}			
}