/************************************************************************************

Filename    :   GoombaController.cs
Content     :   Controller for goomba enemy
Created     :   28 May 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class GoombaController : EnemyController {
	public AudioClip jumpAudioClip;
	public AudioClip stepAudioClip;
	public float squashTimeExtension = 3;

	private bool startFollowPlayer;
	private bool squashed = false;

	protected override void Start() {
		startFollowPlayer = false;
		squashed = false;
		base.Start ();
	}

	protected void OnCollisionEnter(Collision col) {
		if (col.gameObject.name == player.name) {
			base.Knockback(this.gameObject, player);
		} else if (col.gameObject.name == "LeftHandCollider" ||
		           col.gameObject.name == "RightHandCollider" ||
                   col.gameObject.GetComponentInParent<RigidHand>()) {
			base.Knockback(player, this.gameObject, col.gameObject);
		} else if (col.gameObject.tag == "Grabbable" || col.gameObject.tag == "Enemy") {
			base.Knockback(col.gameObject, this.gameObject);
		} else if (col.gameObject.name == "LeftFootCollider" ||
		           col.gameObject.name == "RightFootCollider") {
			if (!squashed) {
				StartCoroutine (Squash ());
			}
		}
	}

	protected override void FollowPlayer() {
		if (!startFollowPlayer) {
				StartCoroutine (Jumped ());
		} else {
			base.FollowPlayer ();
			if (!GetComponent<AudioSource>().isPlaying) {
				GetComponent<AudioSource>().clip = stepAudioClip;
				GetComponent<AudioSource>().Play();
			}
		}
	}

	protected IEnumerator Jumped () {
		GetComponent<Animation>().Play ("Jump");
		yield return new WaitForSeconds(GetComponent<Animation>() ["Jump"].length);
		GetComponent<AudioSource>().clip = jumpAudioClip;
		GetComponent<AudioSource>().Play();
		GetComponent<Animation>().Play ("Walk");
		startFollowPlayer = true;
	}

	protected IEnumerator Squash () {
		squashed = true;
		ReboundPlayer (true);
		GetComponent<Rigidbody>().detectCollisions = false;
		movement = Movement.Freeze;
		GetComponent<Animation>().Play ("Squash");
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		yield return new WaitForSeconds(GetComponent<Animation>() ["Squash"].length + squashTimeExtension);
		dead = true;
        ToggleVisibility ();
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		StartCoroutine(Death(0));
	}
}
