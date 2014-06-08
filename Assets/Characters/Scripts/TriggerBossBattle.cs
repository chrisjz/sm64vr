using UnityEngine;
using System.Collections;

public class TriggerBossBattle : MonoBehaviour {
	public GameObject boss;
	public GameObject worldTheme;
	public AudioClip bossAudioClip;

	private bool bossAudioPlaying;

	void Start () {
		bossAudioPlaying = false;
	}

	void OnTriggerEnter (Collider col) {
		if (col.tag == "Player" && !bossAudioPlaying) {
			worldTheme.audio.clip = bossAudioClip;
			worldTheme.audio.Play ();
			bossAudioPlaying = true;
		}
	}

	protected void SetAudioPlaying(bool state) {
		bossAudioPlaying = state;
	}
}
