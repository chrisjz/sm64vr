/************************************************************************************

Filename    :   CubeMenu.cs
Content     :   3D cube shaped in-game menu
Created     :   11 August 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class CubeMenu : MonoBehaviour {
	public string headerText;
	public string initialMenuName = "Scene";									// Name of first menu shown when opening menu dialog.
	public string exitSceneName;												// Scene where player is sent when they exit.
	public float flipMenuDuration = 0.5f;											// Duration in seconds of when menu flips between sub-menus.
	public SixenseButtons sixenseLeftHandMenuButton = SixenseButtons.THREE;
	public SixenseButtons sixenseRightHandMenuButton = SixenseButtons.FOUR;

	protected SceneManager sceneManager;
	protected GameObject cubeMenu;
	protected string currentActiveMenu;

	void Awake () {
		Transform menuSceneDescription = transform.Find("CubeMenu/Scene/Description");
		sceneManager = (SceneManager) FindObjectOfType (typeof(SceneManager));
		
		if (menuSceneDescription && sceneManager) {
			TextMesh menuSceneDescriptionText = menuSceneDescription.GetComponent<TextMesh> ();
			menuSceneDescriptionText.text = sceneManager.menuSceneDescriptionText;
		}

		cubeMenu = transform.Find ("CubeMenu").gameObject;

		cubeMenu.SetActive (false);
	}

	void Start () {
		currentActiveMenu = initialMenuName;
	}

	void Update () {
		UpdateInput ();
	}

	protected void UpdateInput () {
		// Keyboard
		if (Input.GetKeyUp(KeyCode.Escape)) {
			TriggerVisibility();
		}

		// Sixense
		SixenseInput.Controller hydraLeftController = SixenseInput.GetController (SixenseHands.LEFT);
		SixenseInput.Controller hydraRightController = SixenseInput.GetController (SixenseHands.RIGHT);
		
		if ((hydraLeftController != null && hydraLeftController.GetButtonUp(sixenseLeftHandMenuButton)) ||
		    (hydraRightController != null && hydraRightController.GetButtonUp(sixenseRightHandMenuButton))) {
			TriggerVisibility();
		}
	}
	
	public void HandleButtonByName (string name) {
		switch (name) {
		case "COURSE_CONTINUE":
			Close ();
			break;
		case "COURSE_EXIT":
			StartCoroutine(FlipToActiveMenu ("Exit"));
			break;
		case "EXIT_BACK":
			StartCoroutine(FlipToActiveMenu ("Scene"));
			break;
		case "EXIT_EXIT":
			Exit ();
			break;
		}
	}

	protected void SetActiveMenu (string name, bool enabled) {
		GameObject activeMenu = transform.Find ("CubeMenu/" + name).gameObject;
		
		activeMenu.SetActive(enabled);
		
		if (enabled) {
			currentActiveMenu = name;
		}
	}
	
	protected IEnumerator FlipToActiveMenu (string name) {
		SetActiveMenu (currentActiveMenu,false);
		yield return new WaitForSeconds(flipMenuDuration);
		SetActiveMenu (name, true);
	}

	protected void Close() {
		TriggerVisibility();
	}
	
	protected void Exit() {		
		if (exitSceneName != null) {
			Application.LoadLevel(exitSceneName);
		}
	}

	protected void TriggerVisibility () {
		bool isCubeMenuActive = cubeMenu.activeSelf;

		if (!isCubeMenuActive) {
			SetActiveMenu(currentActiveMenu, false);
			SetActiveMenu(initialMenuName, true);
		}

		cubeMenu.SetActive(!isCubeMenuActive);
	}
}
