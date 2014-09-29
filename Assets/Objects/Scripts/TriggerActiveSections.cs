using UnityEngine;
using System.Collections;

public class TriggerActiveSections : MonoBehaviour {
	public GameObject[] loadSections;
	public GameObject[] unloadSections;
	public GameObject loadAudio;
	public GameObject unloadAudio;
	
	private AudioSource loadAudioSource;
	private AudioSource unloadAudioSource;

	void Awake() {
		if (loadAudio) {
			loadAudioSource = loadAudio.audio;
		}
		if (unloadAudio) {
			unloadAudioSource = unloadAudio.audio;
		}
	}

	// Load or unload areas in scene when player enters trigger
	protected void OnTriggerEnter () {
        TriggerSection ();
	}

    public virtual void TriggerSection () {
        if (loadSections.Length > 0) {
            TriggerSectionActiveness(loadSections, true);
        }
        
        if (unloadSections.Length > 0) {
            TriggerSectionActiveness(unloadSections, false);
        }
        
        if (unloadAudioSource) {
            unloadAudioSource.audio.Stop();
        }
        
        if (loadAudioSource) {
            loadAudio.audio.loop = true;
            loadAudio.audio.Play();
        }
    }

	private void TriggerSectionActiveness(GameObject[] sections, bool isActive) {
		foreach (GameObject section in sections) {
			section.SetActive (isActive);
		}		
	}
}
