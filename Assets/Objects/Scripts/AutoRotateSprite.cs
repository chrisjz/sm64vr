/************************************************************************************

Filename    :   AutoRotateSprite.cs
Content     :   Rotate object to always face the player's direction
Created     :   7 May 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class AutoRotateSprite : MonoBehaviour {
	private GameObject player;

    void Start () {
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
