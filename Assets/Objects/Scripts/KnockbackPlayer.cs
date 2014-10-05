/************************************************************************************

Filename    :   KnockbackPlayer.cs
Content     :   Object which knocks back and damages player
Created     :   7 June 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class KnockbackPlayer : MonoBehaviour {
	public int playerDamage = 1;			// Amount of damage player receives when hit by enemy
	public float knockbackVelocity = 30;	// Distance of how much a victim is knocked back on collision with enemy

	protected GameObject player;
	protected PlayerHealth playerHealth;

	void Awake () {		
		player = GameObject.FindWithTag("Player");
		playerHealth = player.GetComponent<PlayerHealth> ();
	}
	
	protected void OnCollisionEnter(Collision col) {
		if (!player && !playerHealth) {
			return;
		}

		if (col.gameObject.name == player.name) {
			Knockback(this.gameObject, player);
		}
	}
	
	protected virtual void Knockback(GameObject knocker, GameObject victim) {
		Vector3 dir = (victim.transform.position - knocker.transform.position).normalized;
		dir.y = 0;

		CharacterMotor charMotor = player.GetComponent<CharacterMotor> ();
		charMotor.SetVelocity (dir * knockbackVelocity);
		playerHealth.Damage(playerDamage);
	}
}
