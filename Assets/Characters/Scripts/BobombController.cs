using UnityEngine;
using System.Collections;

public class BobombController : EnemyController {
	public float deathTimer = 5; // Seconds until bobomb explodes
	public AudioClip explosionAudioClip;

	protected override void FollowPlayer() {
		base.FollowPlayer ();

		if (deathTimer <= 0) {
			audio.clip = explosionAudioClip;
			audio.Play();
			StartCoroutine(Death(explosionAudioClip.length));
		} else {
			deathTimer -= Time.deltaTime;
		}
	}
}
