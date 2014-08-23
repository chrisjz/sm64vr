/************************************************************************************

Filename    :   GrabObject.cs
Content     :   Spatial positioning of object when grabbed.
Created     :   6 May 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class GrabObject : MonoBehaviour {
	public bool isEnabled = true;
	public bool invertRightHand = true; 						// invert orientation of grabbed object when held by right hand
	public bool disableRigidbodyWhenHeld = true;				// Disable the object's rigidbody when held will decrease jitter in object
	public float additionalThrowForce = 0;						// Additional force added to object when thrown
	public Vector3 positionModifier = new Vector3 (0, 0, 0);
	public Vector3 rotationModifier = new Vector3 (0, 0, 0);
	public Vector3 invertPositionModifier = new Vector3 ();
	public Vector3 invertRotationModifier = new Vector2 ();
	
	private Vector3 defaultPositionModifier = new Vector3 (0, 0, 0);
	private Vector3 defaultRotationModifier = new Vector3 (-180, 180, 0);
	
	public Vector3 GetPosition(SixenseHands Hand) {
		if ( Hand == SixenseHands.LEFT || (Hand == SixenseHands.RIGHT && !invertRightHand)) {
			return positionModifier;
		}
		
		if (Hand == SixenseHands.RIGHT) {
			return positionModifier + invertPositionModifier;
		}
		
		return defaultPositionModifier;
	}

	public Vector3 GetRotation(SixenseHands Hand) {
		if ( Hand == SixenseHands.LEFT || (Hand == SixenseHands.RIGHT && !invertRightHand) ) {
			return rotationModifier;
		}

		if (Hand == SixenseHands.RIGHT) {
			return rotationModifier + invertRotationModifier;
		}
		
		return defaultRotationModifier;
	}

	// Set rigidbody collision detection 
	public void SetRigidbodyDetectionCollisions (bool state) {
		if (rigidbody && disableRigidbodyWhenHeld) {
			rigidbody.detectCollisions = state;
		}
	}
}
