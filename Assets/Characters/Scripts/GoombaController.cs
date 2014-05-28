using UnityEngine;
using System.Collections;

public class GoombaController : EnemyController {
	public AudioClip jumpAudioClip;
	public AudioClip stepAudioClip;

	private bool startFollowPlayer;

	protected override void Start() {
		startFollowPlayer = false;
		base.Start ();
	}

	protected override void FollowPlayer() {
		if (!startFollowPlayer) {
				animation.Play ("Jump");
				StartCoroutine (Jumped (animation ["Jump"].length));
		} else {
			base.FollowPlayer ();
			if (!audio.isPlaying) {
				audio.clip = stepAudioClip;
				audio.Play();
			}
		}
	}

	protected IEnumerator Jumped (float length) {
		yield return new WaitForSeconds(length);
		audio.clip = jumpAudioClip;
		audio.Play();
		animation.Play ("Walk");
		startFollowPlayer = true;
	}
}
