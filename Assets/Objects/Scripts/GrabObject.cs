using UnityEngine;
using System.Collections;

public class GrabObject : MonoBehaviour {
	public bool isEnabled = true;
	public Vector3 positionModifier = new Vector3 (0, 0, 0);
	public Vector3 rotationModifier = new Vector3 (0, 0, 0);
	
	private Vector3 defaultPositionModifier = new Vector3 (0, 0, 0);
	private Vector3 defaultRotationModifier = new Vector3 (0, 0, 0);
	
	public Vector3 GetPosition(SixenseHands Hand) {
		if ( Hand == SixenseHands.LEFT || Hand == SixenseHands.RIGHT ) {
			return positionModifier;
		}
		
		return defaultPositionModifier;
	}

	public Vector3 GetRotation(SixenseHands Hand) {
		if ( Hand == SixenseHands.LEFT || Hand == SixenseHands.RIGHT ) {
			return rotationModifier;
		}
		
		return defaultRotationModifier;
	}
}
