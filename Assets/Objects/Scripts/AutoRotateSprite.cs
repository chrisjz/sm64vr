using UnityEngine;
using System.Collections;

public class AutoRotateSprite : MonoBehaviour {
	public float offsetY = 0;
	private GameObject player;
	private Vector3 rotationVector;

	void Awake () {
		rotationVector = this.transform.rotation.eulerAngles;
		player = GameObject.FindWithTag("Player");
	}

	// Rotate object to always face same direction as player.
	void Update () {
		if (player) {
			rotationVector.y = player.transform.rotation.eulerAngles.y + offsetY;
			this.transform.rotation = Quaternion.Euler(rotationVector);
		}
	}
}
