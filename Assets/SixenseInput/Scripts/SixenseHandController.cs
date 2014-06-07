
//
// Copyright (C) 2013 Sixense Entertainment Inc.
// All Rights Reserved
//

using UnityEngine;
using System.Collections;

public class SixenseHandController : SixenseObjectController
{
	public float					minGrabDistance = 1.0f;
	public float					throwForce = 30.0f;			// Force multiplyer for throwing objects

	private bool					isHoldingObject = false;
	private GameObject				closestObject = null;
	private GrabObject 				grabObject; 				// Script attached to grabbed object with grappling data on that object
	private float					handVelocity;
	private Vector3					handVector;
	private Vector3					handPrevious;

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
		Vector3 currentPosition = new Vector3();
		Quaternion currentRotation = new Quaternion();

		Velocity();
		
		if (isHoldingObject && !controller.GetButton(SixenseButtons.TRIGGER)) {
			Throw();

			isHoldingObject = false;
		}

		if (Hand == SixenseHands.LEFT) {
			currentPosition = GameObject.Find("LeftHandCollider").transform.position;
			currentRotation = GameObject.Find("LeftHandCollider").transform.rotation;
		}
		if (Hand == SixenseHands.RIGHT) {
			currentPosition = GameObject.Find("RightHandCollider").transform.position;
			currentRotation = GameObject.Find("RightHandCollider").transform.rotation;
		}
		
		if (!isHoldingObject) {
			foreach (GameObject o in GameObject.FindGameObjectsWithTag ("Grabbable"))	
			{	
				float dist = Vector3.Distance(o.transform.position, currentPosition);
				if (dist < minGrabDistance)	{	
					closestObject = o;
				}
			}
		}

		if (closestObject != null && Vector3.Distance(closestObject.transform.position, currentPosition) < minGrabDistance && controller.GetButton(SixenseButtons.TRIGGER)) {
			if (closestObject.rigidbody && closestObject.rigidbody.isKinematic) {
				return;
			}

			grabObject = closestObject.GetComponent<GrabObject>();
			if (grabObject && grabObject.isEnabled) {
				closestObject.transform.position = currentPosition + grabObject.GetPosition(Hand);
				closestObject.transform.rotation = currentRotation * Quaternion.Euler(grabObject.GetRotation(Hand));
			} else {
				closestObject.transform.position = currentPosition;
				closestObject.transform.rotation = currentRotation;
			}

			isHoldingObject = true;
		}
	}

	protected void Velocity () {
		if (Time.deltaTime != 0)
		{
			handVector = (transform.position - handPrevious) / Time.deltaTime;
			handPrevious = transform.position; 
		}

		handVelocity = Vector3.Magnitude(handVector);
	}

	// Throw the held object once player lets go based on hand velocity
	protected void Throw () {
		if (closestObject.rigidbody) {			
			Vector3 dir = (closestObject.transform.position - transform.position).normalized;

			closestObject.rigidbody.AddForce(dir * handVelocity * throwForce);
		}
	}
	
	public GameObject GetClosestObject() {
		return closestObject;
	}
	
	public float GetHandVelocity() {
		return handVelocity;
	}

	public bool IsHoldingObject() {
		return isHoldingObject;
	}
}
