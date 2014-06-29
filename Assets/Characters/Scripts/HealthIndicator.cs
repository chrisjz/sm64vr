/************************************************************************************

Filename    :   HealthIndicator.cs
Content     :   Displays player's current health on a physical object
Created     :   29 June 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class HealthIndicator : MonoBehaviour {
	public GameObject player;
	public Texture[] healthTextures;			// Place images of different health increments starting from lowest to highest.

	private PlayerHealth playerHealth;

	void Awake () {
		playerHealth = player.GetComponent<PlayerHealth> ();
	}

	void Update () {
		int health = playerHealth.health;
		Material material = renderer.materials[1];

		if (health < 0) {
			material.mainTexture = healthTextures[0];
		} else if (health + 1 > healthTextures.Length) {
			material.mainTexture = healthTextures[healthTextures.Length];
		} else {
			material.mainTexture = healthTextures[health];
		}
	}
}
