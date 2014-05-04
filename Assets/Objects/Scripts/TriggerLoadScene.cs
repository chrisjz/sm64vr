using UnityEngine;
using System.Collections;

public class TriggerLoadScene : MonoBehaviour {
	public string sceneName;

	protected void OnTriggerEnter () {
		Debug.Log (1);
		if (sceneName != null) {
			Debug.Log (2);
			Application.LoadLevel(sceneName);
		}
	}
}
