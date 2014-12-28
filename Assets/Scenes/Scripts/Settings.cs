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
    public GameObject objectLeapVR;
    public GameObject objectDisplayHealth;
    public GameObject objectDisplayCoins;
	
    protected UIToggle toggleEnableRift;
    protected UIToggle toggleRiftPosTrackMove;
    protected UIToggle toggleRiftPosTrackJump;
    protected UIToggle toggleLeapVR;
    protected UIToggle toggleDisplayHealth;
    protected UIToggle toggleDisplayCoins;

	protected void Awake () {
		GameData.current = new GameData();
#if !UNITY_WEBPLAYER
		StorageManager.Load ();
#endif
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
        if (objectLeapVR) {
            toggleLeapVR = objectLeapVR.GetComponent<UIToggle>();
            toggleLeapVR.value = StorageManager.data.optionControlsLeapVR;
            EventDelegate.Add(toggleLeapVR.onChange, UIToggleLeapVR);
        }
        if (objectDisplayHealth) {
            toggleDisplayHealth = objectDisplayHealth.GetComponent<UIToggle>();
            toggleDisplayHealth.value = StorageManager.data.optionInterfaceDisplayHealth;
            EventDelegate.Add(toggleDisplayHealth.onChange, UIToggleDisplayHealth);
        }
        if (objectDisplayCoins) {
            toggleDisplayCoins = objectDisplayCoins.GetComponent<UIToggle>();
            toggleDisplayCoins.value = StorageManager.data.optionInterfaceDisplayCoins;
            EventDelegate.Add(toggleDisplayCoins.onChange, UIToggleDisplayCoins);
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
    
    public void UIToggleLeapVR () {
        StorageManager.data.optionControlsLeapVR = toggleLeapVR.value;

        GameObject player = GameObject.FindGameObjectWithTag ("Player");

        if (!player) {
            return;
        }

        LeapHandControllerExtender controller = player.GetComponentInChildren<LeapHandControllerExtender> ();
        if (controller) {
            controller.isOrientationSet = false;
            controller.SetOrientation();
        }
    }
    
    public void UIToggleDisplayHealth () {
        StorageManager.data.optionInterfaceDisplayHealth = toggleDisplayHealth.value;
        
        GameObject player = GameObject.FindGameObjectWithTag ("Player");
        
        if (!player) {
            return;
        }
        
        HealthIndicator healthIndicator = player.GetComponentInChildren<HealthIndicator> ();
        if (healthIndicator) {
            healthIndicator.SetPosition();
        }
    }
    
    public void UIToggleDisplayCoins () {
        StorageManager.data.optionInterfaceDisplayCoins = toggleDisplayCoins.value;
        
        GameObject player = GameObject.FindGameObjectWithTag ("Player");
        
        if (!player) {
            return;
        }
        
        CoinIndicator coinIndicator = player.GetComponentInChildren<CoinIndicator> ();
        if (coinIndicator) {
            coinIndicator.SetPosition();
        }
    }
}
