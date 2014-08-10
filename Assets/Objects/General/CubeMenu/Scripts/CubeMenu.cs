using UnityEngine;
using System.Collections;

public class CubeMenu : MonoBehaviour {
	public string exitSceneName;	// Scene where player is sent when they exit.
	private bool visibility;

	void Start () {
		Close ();
	}

	void Update () {
		if (Input.GetKeyUp(KeyCode.Escape)) {
			visibility = !visibility;
			TriggerVisibility(visibility);
		}

	}
	
	public void HandleButtonByName (string name) {
		switch (name) {
		case "COURSE_CONTINUE":
			Close ();
			break;
		case "COURSE_EXIT":
			Exit ();
			break;
		}
	}

	protected void Close() {
		visibility = false;
		TriggerVisibility(visibility);
	}

	protected void Exit() {		
		if (exitSceneName != null) {
			Application.LoadLevel(exitSceneName);
		}
	}

	protected void TriggerVisibility (bool visibility) {
		Transform[] children = gameObject.GetComponentsInChildren<Transform>();

		foreach(Transform child in children) {
			if (child.renderer) {
				child.renderer.enabled = visibility;
			}
		}
	}
}
