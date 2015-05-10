/************************************************************************************

Filename    :   TriggerBossBattle.cs
Content     :   Initiate boss battle when player enters specific zone
Created     :   9 June 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class TriggerBossBattle : MonoBehaviour {
	public GameObject boss;
	public GameObject worldTheme;
	public AudioClip bossAudioClip;

	private BossController bossController;
	private bool bossAudioPlaying;

	void Awake () {
		bossController = boss.GetComponent<BossController> ();
	}

	void Start () {
		bossAudioPlaying = false;
	}

	void OnTriggerEnter (Collider col) {
		if (col.tag == "Player" && !bossAudioPlaying) {
			worldTheme.GetComponent<AudioSource>().clip = bossAudioClip;
			worldTheme.GetComponent<AudioSource>().Play ();
			bossAudioPlaying = true;
			bossController.SetInitBattle(true);
		}
	}

	public void SetAudioPlaying(bool state) {
		bossAudioPlaying = state;
	}
}
