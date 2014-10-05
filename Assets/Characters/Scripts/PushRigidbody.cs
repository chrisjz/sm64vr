/************************************************************************************

Filename    :   PushRigidbody.cs
Content     :   
Created     :   7 June 2014
Authors     :   titegtnodI
Source      :   Unity3D Answers

************************************************************************************/

// Credit: titegtnodI @ Unity3D Answers

using UnityEngine;
using System.Collections;

public class PushRigidbody : MonoBehaviour {
	public float pushPower = 2.0f;
	public float weight = 2.0f;

	protected void OnControllerColliderHit (ControllerColliderHit hit) {
		Rigidbody body = hit.collider.attachedRigidbody;
		Vector3 force;
		
		// no rigidbody
		if (body == null || body.isKinematic) {
			return;
		}

		// We use gravity and weight to push things down, we use
		// our velocity and push power to push things other directions
		if (hit.moveDirection.y < -0.3) {
			force = new Vector3 (0f, -0.5f, 0f) * weight;
		} else {
			force = hit.controller.velocity * pushPower;
		}

		// Apply the push
		body.AddForceAtPosition(force, hit.point);
	}
}
