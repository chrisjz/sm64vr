/************************************************************************************

Filename    :   Settings.cs
Content     :   Game settings handler
Created     :   21 September 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour {
	public GameObject objectEnableRift;
	public GameObject objectRiftPosTrackMove;
	public GameObject objectRiftPosTrackJump;
	
	protected UIToggle toggleEnableRift;
	protected UIToggle toggleRiftPosTrackMove;
	protected UIToggle toggleRiftPosTrackJump;

	protected void Awake () {
		GameData.current = new GameData();
		StorageManager.Load ();
	}

	protected void Start () {
		InitToggles();
	}

	protected void InitToggles () {
		if (objectEnableRift) {
			toggleEnableRift = objectEnableRift.GetComponent<UIToggle>();
			toggleEnableRift.value = StorageManager.data.optionControlsEnableRift;
			EventDelegate.Add(toggleEnableRift.onChange, UIToggleEnableRift);
		}
		if (objectRiftPosTrackMove) {
			toggleRiftPosTrackMove = objectRiftPosTrackMove.GetComponent<UIToggle>();
			toggleRiftPosTrackMove.value = StorageManager.data.optionControlsRiftPosTrackMove;
			EventDelegate.Add(toggleRiftPosTrackMove.onChange, UIToggleRiftPosTrackMove);
		}
		if (objectRiftPosTrackJump) {
			toggleRiftPosTrackJump = objectRiftPosTrackJump.GetComponent<UIToggle>();
			toggleRiftPosTrackJump.value = StorageManager.data.optionControlsRiftPosTrackJump;
			EventDelegate.Add(toggleRiftPosTrackJump.onChange, UIToggleRiftPosTrackJump);
		}
	}

	public void Save () {
		StorageManager.Save ();
	}
	
	public void UIToggleEnableRift () {
		StorageManager.data.optionControlsEnableRift = toggleEnableRift.value;
	}
	
	public void UIToggleRiftPosTrackMove () {
		StorageManager.data.optionControlsRiftPosTrackMove = toggleRiftPosTrackMove.value;
	}
	
	public void UIToggleRiftPosTrackJump () {
		StorageManager.data.optionControlsRiftPosTrackJump = toggleRiftPosTrackJump.value;
	}
}
