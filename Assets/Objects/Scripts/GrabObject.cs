using UnityEngine;
using System.Collections;

public class GrabObject : MonoBehaviour {
	public bool enabled = true;
	public Vector3 positionModifier;
	
	private Vector3 defaultPositionModifier = new Vector3 (0, 0, 0);
	
	public Vector3 GetPosition(SixenseHands Hand) {		
		if ( Hand == SixenseHands.LEFT || Hand == SixenseHands.RIGHT ) {
			return positionModifier;
		}
		
		return defaultPositionModifier;
	}
}
