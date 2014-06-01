using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
	public int maxHealth = 8;						// Maximum amount of health that player can have
	public int health = 8;							// Player's current health
	public float recoveryInvincibilityTime = 3;		// Amount of time that player is invincible after being hurt
	public AudioClip hurtAudioClip;

	private int initHealth;							// Stores initial health at start of level to handle respawning
	private bool beingDamaged;						// True if player is currently being damaged

	void Start () {
		initHealth = health;
		beingDamaged = false;
	}

	void Update () {
		if (health <= 0) {
			Death();
		}
	}

	public void Damage (int damage) {
		if (!beingDamaged) {
			beingDamaged = true;
			health -= damage;
			audio.clip = hurtAudioClip;
			audio.Play();
			Debug.Log ("Player health is " + health + "/" + maxHealth);
			StartCoroutine (Invincibility (recoveryInvincibilityTime));
		}
	}

	protected IEnumerator Invincibility (float time) {
		yield return new WaitForSeconds(time);
		beingDamaged = false;
	}

	protected void Death() {
		Debug.Log ("Player has died");
	}
}
