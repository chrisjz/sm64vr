
//
// Copyright (C) 2013 Sixense Entertainment Inc.
// All Rights Reserved
//

using UnityEngine;
using System.Collections;

public class SixenseHandController : SixenseObjectController
{
	static GameObject			closestObject = null;
	protected bool				holdingObject = false;
	
	protected override void Start() 
	{		
		base.Start();
	}
	
	protected override void UpdateObject( SixenseInput.Controller controller )
	{
		
		if ( controller.Enabled )  
		{		
			// Animation update
			UpdateAnimationInput( controller );

			// Action update
			UpdateActionInput ( controller );
		}
				
		base.UpdateObject(controller);
	}
	
	
	void OnGUI()
	{
		if ( Hand == SixenseHands.UNKNOWN )
		{
			return;
		}
		
		if ( !m_enabled )
		{
			int labelWidth = 250;
			int labelPadding = 120;
			int horizOffset = Hand == SixenseHands.LEFT ? -labelWidth - labelPadding  : labelPadding;
			
			string handStr = Hand == SixenseHands.LEFT ? "left" : "right";
			GUI.Box( new Rect( Screen.width / 2 + horizOffset, Screen.height - 40, labelWidth, 30 ),  "Press " + handStr + " START to control " + gameObject.name );		
		}		
	}
	
	// Updates the animated object from controller input.
	protected void UpdateAnimationInput( SixenseInput.Controller controller){}

	protected void UpdateActionInput( SixenseInput.Controller controller) {
		float minDist = 1.0f;	
		Vector3 currentPosition = new Vector3();
		Quaternion currentRotation = new Quaternion();
		
		if (holdingObject && !controller.GetButton(SixenseButtons.TRIGGER)) {
			holdingObject = false;
		}

		if (Hand == SixenseHands.LEFT) {
			currentPosition = GameObject.Find("LeftHandCollider").transform.position;
			currentRotation = GameObject.Find("LeftHandCollider").transform.rotation;
			//Debug.Log ("left"+currentPosition+currentRotation);
		}
		if (Hand == SixenseHands.RIGHT) {
			currentPosition = GameObject.Find("RightHandCollider").transform.position;
			currentRotation = GameObject.Find("RightHandCollider").transform.rotation;
			//Debug.Log ("right"+currentPosition+currentRotation);
		}
		
		if (!holdingObject) {
			foreach (GameObject o in GameObject.FindGameObjectsWithTag ("Grabbable"))	
			{	
				float dist = Vector3.Distance(o.transform.position, currentPosition);
				//Debug.Log ("dist"+dist);
				
				if (dist < minDist)	{	
					closestObject = o;	
					minDist = dist;	
				}
			}
		}
		
		if (closestObject != null && Vector3.Distance(closestObject.transform.position, currentPosition) < 1.0 && controller.GetButton(SixenseButtons.TRIGGER)) {
			if (closestObject.rigidbody && closestObject.rigidbody.isKinematic)
				return;
			
			closestObject.transform.position = currentPosition;
			closestObject.transform.rotation = currentRotation;
			holdingObject = true;
		}
	}
	
	public static GameObject getClosestObject() {
		return closestObject;
	}
}
