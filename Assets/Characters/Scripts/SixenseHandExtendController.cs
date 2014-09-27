/************************************************************************************

Filename    :   SixenseHandExtendController.cs
Content     :   Extend Sixense Hand Controller
Created     :   25 June 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class SixenseHandExtendController : SixenseHandController {
    public float                    minGrabDistance = 1.0f;
    public float					throwForce = 30.0f;			// Force multiplier for throwing objects

    protected bool                  isHoldingObject = false;
    protected GameObject            closestObject = null;
    protected GrabObject            grabObject; 				// Script attached to grabbed object with grappling data on that object
    protected float                 handVelocity;
    protected Vector3               handDirection;
    protected Vector3               handVector;
    protected Vector3               handPreviousPosition;
    protected Vector3               handPreviousDirection;
	
	protected override void Start() 
	{
		// get the Animator
		m_animator = this.gameObject.GetComponent<Animator>();
		
		base.Start();
	}
	
	protected override void UpdateObject( SixenseInput.Controller controller )
	{		
		if ( controller.Enabled )  
		{		
			// Action update
			UpdateActionInput ( controller );
		}
		
		base.UpdateObject(controller);
	}
	
	
	protected void UpdateActionInput( SixenseInput.Controller controller) {
		Vector3 currentPosition = GetCurrentColliderPosition ();
		Quaternion currentRotation = GetCurrentColliderRotation ();
		
		Velocity();
		
		if (isHoldingObject && !controller.GetButton(SixenseButtons.TRIGGER)) {
			Throw();
			
			isHoldingObject = false;
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
		
		if ((isHoldingObject && controller.GetButton(SixenseButtons.TRIGGER)) ||
				(closestObject != null && Vector3.Distance(closestObject.transform.position, currentPosition) < minGrabDistance && controller.GetButton(SixenseButtons.TRIGGER))) {
			if (closestObject.rigidbody && closestObject.rigidbody.isKinematic) {
				return;
			}
			
			grabObject = closestObject.GetComponent<GrabObject>();
			if (grabObject && grabObject.isEnabled) {
				grabObject.SetRigidbodyDetectionCollisions(false);
				closestObject.transform.position = currentPosition + closestObject.transform.TransformDirection(grabObject.GetPosition(Hand));
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
			handDirection = transform.position - handPreviousPosition;
			handVector = (transform.position - handPreviousPosition) / Time.deltaTime;
			handPreviousPosition = transform.position;
		}
		
		handVelocity = Vector3.Magnitude(handVector);
	}
	
	// Throw the held object once player lets go based on hand velocity
	// TODO: Take into account weight (1 unit = kilogram) of held object
	protected void Throw () {
		if (closestObject.rigidbody) {
			grabObject = closestObject.GetComponent<GrabObject>();
			float additionalThrowForce = 0;
			
			if (grabObject) {
                additionalThrowForce += grabObject.additionalThrowForce;
                grabObject.SetRigidbodyDetectionCollisions(true);
			}
			
			closestObject.rigidbody.AddForce(handDirection * handVelocity * (throwForce + additionalThrowForce));
		}
	}
	
	public Vector3 GetCurrentColliderPosition () {		
		if (Hand == SixenseHands.LEFT) {
			return GameObject.Find ("LeftHandCollider").transform.position;
		} else if (Hand == SixenseHands.RIGHT) {
			return GameObject.Find ("RightHandCollider").transform.position;
		} else {
			return new Vector3();
		}
	}
	
	public Quaternion GetCurrentColliderRotation () {		
		if (Hand == SixenseHands.LEFT) {
			return GameObject.Find ("LeftHandCollider").transform.rotation;
		} else if (Hand == SixenseHands.RIGHT) {
			return GameObject.Find ("RightHandCollider").transform.rotation;
		} else {
			return new Quaternion();
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
