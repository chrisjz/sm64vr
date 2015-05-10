/************************************************************************************

Filename    :   BobombController.cs
Content     :   Controller for bobomb enemy
Created     :   14 May 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class BobombController : EnemyController {
	public float deathTimer = 5; 			// Seconds until bobomb explodes
	public float damageRadius = 5; 			// Radius where objects get damaged from source of bobomb's explosion
	public float explosionEnemyForce = 10;	// Extra force on knocking back enemies within detonation radius
	public AudioClip explosionAudioClip;
	
	private Transform smoke;
	private GameObject explosion;
	private float defaultDeathTimer;
	
	protected override void Awake() {
		base.Awake ();
		smoke = transform.Find ("Smoke");
		defaultDeathTimer = deathTimer;
	}

	protected void OnCollisionEnter(Collision col) {
		// Bobomb explodes when colliding with something whilst being thrown.
		if (heldByPlayer && !IsHoldingEnemy () && (col.gameObject.name != "LeftHandCollider" ||
		    	col.gameObject.name != "RightHandCollider")) {
			deathTimer = 0;
		}
	}
	
	protected override void Init() {
		deathTimer = defaultDeathTimer;
		base.Init ();
	}
	
	protected override void FollowPlayer() {
		base.FollowPlayer ();
		Detonation ();
	}
	
	protected override void Freeze() {
		base.Freeze ();

		if (!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().clip = followAudioClip;
			GetComponent<AudioSource>().Play();
		}

		Detonation ();
	}

	protected void Detonation () {		
		if (!smoke.GetComponent<ParticleSystem>().isPlaying) {			
			smoke.GetComponent<ParticleSystem>().Play ();
		}
		
		if (deathTimer <= 0) {
			GetComponent<Animation>().Play("Explode");
			StartCoroutine(Explode(GetComponent<Animation>()["Explode"].length));
		} else {
			deathTimer -= Time.deltaTime;
		}
	}

	protected IEnumerator Explode (float length) {
		explosion = (GameObject) Instantiate(Resources.Load("Explosion"));
		explosion.transform.position = transform.position;
		dead = true;
		yield return new WaitForSeconds(length);
		DamageObjectsInRadius ();
		GetComponent<AudioSource>().clip = explosionAudioClip;
		GetComponent<AudioSource>().Play();
		smoke.GetComponent<ParticleSystem>().Stop ();
		ToggleVisibility ();
		StartCoroutine(Death(explosionAudioClip.length));
	}

	protected void DamageObjectsInRadius() {
		DamagePlayersInRadius ();
		DamageEnemiesInRadius ();
	}

	protected void DamagePlayersInRadius() {
		if (player) {
			float dist = Vector3.Distance(player.transform.position, transform.position);
			if (dist < damageRadius)	{	
				base.Knockback(gameObject, player.gameObject);
			}
		}
	}

	protected void DamageEnemiesInRadius() {
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");

		foreach (GameObject enemy in enemies) {
			EnemyController enemyController = enemy.GetComponent<EnemyController>();
			float dist = Vector3.Distance(enemy.transform.position, transform.position);

			if (dist < damageRadius && enemyController)	{	
				enemyController.Knockback(gameObject, enemy.gameObject, null, explosionEnemyForce);
			}
		}
	}
}
