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
    public GameObject objectDisplayHealth;
	
    protected UIToggle toggleEnableRift;
    protected UIToggle toggleRiftPosTrackMove;
    protected UIToggle toggleRiftPosTrackJump;
    protected UIToggle toggleDisplayHealth;

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
        if (objectDisplayHealth) {
            toggleDisplayHealth = objectDisplayHealth.GetComponent<UIToggle>();
            toggleDisplayHealth.value = StorageManager.data.optionInterfaceDisplayHealth;
            EventDelegate.Add(toggleDisplayHealth.onChange, UIToggleDisplayHealth);
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
    
    public void UIToggleDisplayHealth () {
        StorageManager.data.optionInterfaceDisplayHealth = toggleDisplayHealth.value;
    }
}
