using UnityEngine;
using System.Collections;

public class BobombController : EnemyController {
	public float deathTimer = 5; // Seconds until bobomb explodes
	public AudioClip explosionAudioClip;

	protected override void FollowPlayer() {
		base.FollowPlayer ();

		if (deathTimer <= 0) {
			animation.Play("Explode");
			audio.clip = explosionAudioClip;
			audio.Play();
			StartCoroutine(Death(animation.clip.length));
		} else {
			deathTimer -= Time.deltaTime;
		}
	}
}
