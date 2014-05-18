using UnityEngine;
using System.Collections;

public class BobombController : EnemyController {
	public float deathTimer = 5; // Seconds until bobomb explodes
	public AudioClip explosionAudioClip;

	private Transform smoke;

	protected override void Awake() {
		base.Awake ();
		smoke = transform.Find ("Smoke");
	}

	protected override void FollowPlayer() {
		base.FollowPlayer ();

		if (!smoke.particleSystem.isPlaying) {			
			smoke.particleSystem.Play ();
		}

		if (deathTimer <= 0) {
			animation.Play("Explode");
			StartCoroutine(Explode(animation["Explode"].length));
		} else {
			deathTimer -= Time.deltaTime;
		}
	}

	protected IEnumerator Explode (float length) {
		dead = true;
		yield return new WaitForSeconds(length);
		audio.clip = explosionAudioClip;
		audio.Play();
		ToggleVisibility ();
		StartCoroutine(Death(explosionAudioClip.length));
	}
}
