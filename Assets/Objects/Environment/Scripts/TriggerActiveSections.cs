using UnityEngine;
using System.Collections;

public class TriggerActiveSections : MonoBehaviour {
	public GameObject[] loadSections;
	public GameObject[] unloadSections;

	// Load or unload areas in scene when player enters trigger
	protected void OnTriggerEnter () {
		if (loadSections.Length > 0) {
			TriggerSectionActiveness(loadSections, true);
		}

		if (unloadSections.Length > 0) {
			TriggerSectionActiveness(unloadSections, false);
		}
	}

	private void TriggerSectionActiveness(GameObject[] sections, bool isActive) {
		foreach (GameObject section in sections) {
			section.SetActive (isActive);
		}		
	}
}
