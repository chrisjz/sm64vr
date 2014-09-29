/************************************************************************************

Filename    :   Title.cs
Content     :   Scene manager for Title scene
Created     :   15 September 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class Title : MonoBehaviour {
    
    public enum OvrCameras { Left, Right }
    public OvrCameras mainOvrCamera = OvrCameras.Right;                 // OVR Camera where movement is oriented towards, should have audio listener too
    public bool detectOvr = true;                                       // Detects if player is using Oculus Rift

    public GameObject menu;
    public GameObject initialMenuPanel;
    public GameObject titleActionText;
    public float titleActionFlickerSpeed = 1;

    protected float titleActionTimer;
    protected bool titleActionActive;
    
    protected GameObject objectMarioHead;
    protected GameObject objectRift;

    protected GameObject ovrCameraLeft;
    protected GameObject ovrCameraRight;
    protected GameObject generalCamera;                                   // Camera for monoscopic view
    protected GameObject dirOvrCamera;                                    // Movement oriented using this camera for OVR
    protected bool HMDPresent = false;

    protected void Awake () {
        // Objects
        objectMarioHead = GameObject.Find ("Mario Head");
        objectRift = GameObject.Find ("Rift");

        // Cameras
        ovrCameraLeft = GameObject.Find("OVRCameraController/CameraLeft").gameObject;
        ovrCameraRight = GameObject.Find("OVRCameraController/CameraRight").gameObject;
        generalCamera = GameObject.Find("OVRCameraController/Camera").gameObject;

        PlayerPrefs.SetString ("previousSceneName", null);
        PlayerPrefs.SetString ("previousSceneExitAction", null);
    }

    protected void Start () {
        InitCamera();
        objectRift.transform.Find("rift").gameObject.renderer.enabled = false;
        PlayIntroAnimation ();
        menu.SetActive (false);
        initialMenuPanel.SetActive (false);
        titleActionTimer = titleActionFlickerSpeed;
        titleActionActive = true;
	}
	
    protected void Update () {
        if (!titleActionActive) {
            return;
        }

        FlickerActionText ();
        UpdateAction ();
	}

    protected void FlickerActionText() {

        titleActionTimer -= Time.deltaTime;
        if (titleActionTimer < 0) {
            titleActionText.SetActive(!titleActionText.activeSelf);
            titleActionTimer = titleActionFlickerSpeed;
        }
    }

    protected void UpdateAction () {
        // Keyboard
        if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter)) {
            menu.SetActive (true);
            initialMenuPanel.SetActive(true);
            titleActionText.SetActive(false);
            titleActionActive = false;
        }
    }
    
    public void InitCamera() {
        if (mainOvrCamera == OvrCameras.Left) {
            dirOvrCamera = ovrCameraLeft;
        } else {
            dirOvrCamera = ovrCameraRight;
        }
        
        // Apply the direction to the CharacterMotor
        HMDPresent = OVRDevice.IsHMDPresent();
        
        if (detectOvr) {
            DetectOVR();
        }
    }

    protected void PlayIntroAnimation () {
        objectMarioHead.animation.clip = objectMarioHead.animation.GetClip("Intro");
        objectMarioHead.animation.Play ();
        StartCoroutine (PlayIntroAnimation2(objectMarioHead.animation.clip.length));
    }

    protected IEnumerator PlayIntroAnimation2 (float length) {
        yield return new WaitForSeconds(length);
        objectRift.transform.Find("rift").renderer.enabled = true;
        objectRift.animation.clip = objectRift.animation.GetClip("Intro");
        objectRift.animation.Play ();

    }
    
    // Show OVR Camera only if OVR is being used
    protected void DetectOVR() {
        HMDPresent = OVRDevice.IsHMDPresent();

        if (HMDPresent == false || StorageManager.data.optionControlsEnableRift == false) {
            ovrCameraLeft.SetActive(false);
            ovrCameraRight.SetActive(false);
            generalCamera.SetActive(true);
            
            generalCamera.GetComponent<AudioListener>().enabled = true;
            dirOvrCamera.GetComponent<AudioListener>().enabled = false;
        } else {
            ovrCameraLeft.SetActive(true);
            ovrCameraRight.SetActive(true);
            generalCamera.SetActive(false);
            
            generalCamera.GetComponent<AudioListener>().enabled = false;
            dirOvrCamera.GetComponent<AudioListener>().enabled = true;
        }
    }
}
