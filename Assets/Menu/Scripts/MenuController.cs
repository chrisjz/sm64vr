/************************************************************************************

Filename    :   MenuController.cs
Content     :   Controller for player menu
Created     :   23 September 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {
    public GameObject initialMenuPanel;
    public GameObject NGUIRoot;             // Root NGUI object, usually named "UI Root"
    public Vector3 offsetNoOVR = new Vector3();

    protected GameObject menu;
    protected FPSInputController inputController;

    private bool HMDPresent;

    protected void Awake () {
    }

    protected void Start () {
        menu = transform.Find ("Menu").gameObject;
        inputController = transform.GetComponent<FPSInputController> ();
        menu.SetActive (false);        
#if !UNITY_WEBPLAYER
        HMDPresent = OVRManager.display.isPresent;
#else
        HMDPresent = false;
#endif

        SetMenuPosition ();
    }
	
    protected void Update () {
        // Keyboard
        if (Input.GetKeyUp(KeyCode.Escape)) {
            menu.SetActive (!menu.activeSelf);
            if (menu.activeSelf) {
                NGUITools.SetActive(initialMenuPanel, true);
                inputController.enableMovement = false;
            } else {
                foreach (Transform child in NGUIRoot.transform) {
                    if (child.name.StartsWith("Panel")) {
                        child.gameObject.SetActive(false);
                    }
                }
                inputController.enableMovement = true;
            }
        }
	}

    protected void SetMenuPosition () {
        // Change menu position if Rift is disabled
        if (!StorageManager.data.optionControlsEnableRift || !HMDPresent) {
            menu.transform.localPosition += offsetNoOVR;
        }
    }
}
