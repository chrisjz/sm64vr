using UnityEngine;
using System.Collections;

public class TriggerLoadScene : MonoBehaviour {
	public string sceneName;

	protected void OnTriggerEnter () {
		if (sceneName != null) {
			Application.LoadLevel(sceneName);
		}
	}
}
