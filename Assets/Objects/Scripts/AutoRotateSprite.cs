using UnityEngine;
using System.Collections;

public class AutoRotateSprite : MonoBehaviour {
	public GameObject player;
	private Vector3 rotationVector;

	void Awake () {
		rotationVector = this.transform.rotation.eulerAngles;
	}

	// Rotate object to always face same direction as player.
	void Update () {
		if (player) {
			rotationVector.y = player.transform.rotation.eulerAngles.y;
			this.transform.rotation = Quaternion.Euler(rotationVector);
		}
	}
}
