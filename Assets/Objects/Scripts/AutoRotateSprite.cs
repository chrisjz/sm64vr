using UnityEngine;
using System.Collections;

public class AutoRotateSprite : MonoBehaviour {
	private GameObject player;
	private Vector3 rotationVector;

	void Awake () {
		rotationVector = this.transform.rotation.eulerAngles;
		player = GameObject.FindWithTag("Player");
	}

	// Rotate object to always face same direction as player.
	void Update () {
		if (!player) {
			return;
		}

		Vector3 lookPosition = player.transform.position - transform.position;
		lookPosition.y = 0.0f;
		transform.rotation = Quaternion.LookRotation(lookPosition);

	}
}
