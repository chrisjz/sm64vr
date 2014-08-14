/************************************************************************************

Filename    :   CubeMenu.cs
Content     :   3D cube shaped in-game menu
Created     :   11 August 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class CubeMenu : MonoBehaviour {
	public string exitSceneName;	// Scene where player is sent when they exit.
	public SixenseButtons sixenseLeftHandMenuButton = SixenseButtons.THREE;
	public SixenseButtons sixenseRightHandMenuButton = SixenseButtons.FOUR;

	private bool visibility;

	void Start () {
		Close ();
	}

	void Update () {
		if (Input.GetKeyUp(KeyCode.Escape)) {
			visibility = !visibility;
			TriggerVisibility(visibility);
		}


		SixenseInput.Controller hydraLeftController = SixenseInput.GetController (SixenseHands.LEFT);
		SixenseInput.Controller hydraRightController = SixenseInput.GetController (SixenseHands.RIGHT);
		
		if ((hydraLeftController != null && hydraLeftController.GetButtonUp(sixenseLeftHandMenuButton)) ||
		    (hydraRightController != null && hydraRightController.GetButtonUp(sixenseRightHandMenuButton))) {
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
		foreach(Transform trans in transform) {
			if (gameObject.name != trans.gameObject.name) {
				trans.gameObject.SetActive(visibility);
			}
		}
	}
}
