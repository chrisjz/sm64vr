/************************************************************************************

Filename    :   Title.cs
Content     :   Scene manager for Title scene
Created     :   15 September 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Title : MonoBehaviour {
    
    public enum OvrCameras { Left, Right }
    public OvrCameras mainOvrCamera = OvrCameras.Right;                 // OVR Camera where movement is oriented towards
    public bool detectOvr = true;                                       // Detects if player is using Oculus Rift

    public GameObject menu;
    public GameObject initialMenuPanel;
    public GameObject scoreMenuPanel;
    public GameObject titleActionText;
    public float titleActionFlickerSpeed = 1;

    protected float titleActionTimer;
    protected bool titleActionActive;

    // Camera transitions to Mario's viewpoint
    protected GameObject startMarkerTransitionCamera;
    protected GameObject endMarkerTransitionCamera;
    protected float startTimeTransitionCamera;
    protected float journeyLengthTransitionCamera;
    protected float journeyLengthStartTransitionRotation;
    protected float transitionPositionSpeed;
    protected float transitionRotationSpeed;
    protected bool transitionMenu;                                      
    
    protected GameObject objectLogo;
    protected GameObject objectMarioHead;
    protected GameObject objectRift;

    protected GameObject cameraController;
    protected OVRCameraRig ovrCameraRig;
    protected OVRManager ovrManager;
    protected GameObject ovrCameraLeft;
    protected GameObject ovrCameraRight;
    protected GameObject generalCamera;                                 // Camera for monoscopic view
    protected GameObject dirOvrCamera;                                  // Movement oriented using this camera for OVR
    protected bool HMDPresent = false;
    protected bool isDirectToRift = false;

    protected void Awake () {
        // Objects
        objectLogo = GameObject.Find ("Logo");
        objectMarioHead = GameObject.Find ("Mario Head");
        objectRift = GameObject.Find ("Rift");
        
        // Camera rig
        cameraController = GameObject.Find ("CameraController").gameObject;
        ovrCameraRig = cameraController.GetComponentInChildren<OVRCameraRig> ();
        ovrManager = cameraController.GetComponentInChildren<OVRManager> ();

        // Cameras
        ovrCameraLeft = GameObject.Find("OVRCameraRig/LeftEyeAnchor").gameObject;
        ovrCameraRight = GameObject.Find("OVRCameraRig/RightEyeAnchor").gameObject;
        generalCamera = GameObject.Find("OVRCameraRig/MonoEyeAnchor").gameObject;

        // Transition to Mario's viewpoint        
        startMarkerTransitionCamera = new GameObject();
        endMarkerTransitionCamera = new GameObject ();

        PlayerPrefs.SetString ("previousSceneName", null);
        PlayerPrefs.SetString ("previousSceneExitAction", null);
    }

    protected void Start () {
        DetectDirectToRift();
        InitCamera ();
        PrefillScore ();
        objectRift.transform.Find("rift").gameObject.renderer.enabled = false;
        PlayIntroAnimation ();
        menu.SetActive (false);
        initialMenuPanel.SetActive (false);
        titleActionTimer = titleActionFlickerSpeed;
        titleActionActive = true;
        transitionMenu = false;
	}
	
    protected void Update () {
        if (transitionMenu) {
            TransitionToFirstPerson ();
        }

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
            initialMenuPanel.SetActive(true);
            titleActionText.SetActive(false);
            titleActionActive = false;

            Vector3 endMarkerOffset;
            if (generalCamera.activeSelf) {
                endMarkerOffset = new Vector3 (0f, 0f, 5.5f);
                transitionPositionSpeed = 7.0f;
                transitionRotationSpeed = 0.07f;
                journeyLengthStartTransitionRotation = 0.8f;
            } else {
                endMarkerOffset = new Vector3 (0f, 0f, 7.5f);
                transitionPositionSpeed = 5.0f;
                transitionRotationSpeed = 0.07f;
                journeyLengthStartTransitionRotation = 0.5f;
            }

            startMarkerTransitionCamera.transform.position = cameraController.transform.position;
            endMarkerTransitionCamera.transform.position = startMarkerTransitionCamera.transform.position + endMarkerOffset;
            startTimeTransitionCamera = Time.time;
            journeyLengthTransitionCamera = Vector3.Distance(startMarkerTransitionCamera.transform.position, endMarkerTransitionCamera.transform.position);
            transitionMenu = true;
        }
    }
    
    public void InitCamera() {
        if (mainOvrCamera == OvrCameras.Left) {
            dirOvrCamera = ovrCameraLeft;
        } else {
            dirOvrCamera = ovrCameraRight;
        }
        
        if (detectOvr) {
            DetectOVR();
        }
    }

    // Prefill's coin score for each world
    protected void PrefillScore () {
        int[] coins = new int[14];
        if (StorageManager.data.coins != null)
            coins = StorageManager.data.coins;
        else
            StorageManager.data.coins = new int[14];

        for (int key = 0; key < coins.Length; ++key) {
            Transform level = scoreMenuPanel.transform.Find((key + 1) + "/Label - Coins");
            if (level)
                level.GetComponent<UILabel> ().text = coins[key].ToString ();
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

    protected void TransitionToFirstPerson () {
        float distCovered = (Time.time - startTimeTransitionCamera) * transitionPositionSpeed;
        float fracJourney = distCovered / journeyLengthTransitionCamera;
        cameraController.transform.position = Vector3.Lerp(startMarkerTransitionCamera.transform.position, endMarkerTransitionCamera.transform.position, fracJourney);

        if (fracJourney >= journeyLengthStartTransitionRotation) {
            cameraController.transform.rotation = Quaternion.Lerp (cameraController.transform.rotation, Quaternion.Euler(new Vector3 (0, 180.0f, 0)), transitionRotationSpeed);
        }
        
        HMDPresent = OVRManager.display.isPresent;

        if (!isDirectToRift && (!HMDPresent || !StorageManager.data.optionControlsEnableRift)) { 
            if (fracJourney >= 1f) {
                objectLogo.SetActive (false);
            }
        } else if (fracJourney >= 0.5f) {
            objectLogo.SetActive (false);
        }

        if (fracJourney >= 1f) {
            menu.SetActive (true);
        }
    }
    
    // Show OVR Camera only if OVR is being used
    protected void DetectOVR() {
        HMDPresent = OVRManager.display.isPresent;

        if (!isDirectToRift && (!HMDPresent || !StorageManager.data.optionControlsEnableRift)) {
            ovrCameraLeft.SetActive(false);
            ovrCameraRight.SetActive(false);
            generalCamera.SetActive(true);
            ovrCameraRig.enabled = false;
            ovrManager.enabled = false;
        } else {
            ovrCameraLeft.SetActive(true);
            ovrCameraRight.SetActive(true);
            generalCamera.SetActive(false);
            ovrCameraRig.enabled = true;
            ovrManager.enabled = true;
        }
    }    
    
    //Detect if game is launched via Direct to Rift executable.
    // Credit: PhilipRamirez from Oculus Forum
    protected void DetectDirectToRift()
    {
        long exeSize = 0;
        {
            FileInfo exeFile = new System.IO.FileInfo (Environment.GetCommandLineArgs () [0]);   // Path name of the .exe used to launch
            exeSize = exeFile.Length;   // exeFile.Length return the file size in bytes. Store it for comparison
        }

        // Use file to determine which exe was launched. This should be stable even if a user changes the name of the .exe or uses a shortcut! =D
        // Direct Rift sizes: 184320 is 64bit size, 32 is 164864 (3rd check is for extended mode(NOT FULLY TESTED)) 
        // (You may want to use Debug.Log(exeSize); to double check the file size is the same on your match)
        
        if ((exeSize == 184320 || exeSize == 164864)) {
            // DirectToRift.exe
            isDirectToRift = true;
        } else {
            // Standard.exe
            isDirectToRift = false;
        }
    }
}
