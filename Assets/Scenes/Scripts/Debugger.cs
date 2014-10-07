/************************************************************************************

Filename    :   Debugger.cs
Content     :   Debug game
Created     :   10 June 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class Debugger : MonoBehaviour {
    public AudioClip audioEnable;
    public AudioClip audioDisable;

	protected bool enable = false;
    protected StereoDialog stereoDialog;
	
	private GameObject player;
	private KonamiCode konamiCode;
	private string curSceneName;
    private bool prevState = false;     // Previous state of variable "enable"

	void Awake () {
        konamiCode = GetComponent<KonamiCode> ();
        stereoDialog = GameObject.Find ("StereoDialog").GetComponent<StereoDialog> ();
	}
	
	void Start () {		
		player = GameObject.FindWithTag("Player");
		curSceneName = Application.loadedLevelName;
        enable = StorageManager.data.debug;
        prevState = enable;

        if (konamiCode)
            konamiCode.KonamiCodeEnabled = enable;
	}
	
	void Update () {
        // Save debug state changes to storage
        if (prevState != enable) {
            UpdateTriggerAudio(enable);
            StorageManager.data.debug = enable;
            StorageManager.Save();
            prevState = enable;
        }

		// Enable if user enters konami code
		if (konamiCode)
            enable = konamiCode.KonamiCodeEnabled;

		if (!enable)
			return;
		
		RestartScene ();
		Teleport ();
	}

    private void UpdateTriggerAudio (bool enable) {
        if (enable && audioEnable) {
            audio.clip = audioEnable;
            audio.Play();
        }

        if (!enable && audioDisable) {
            audio.clip = audioDisable;
            audio.Play();
        }
    }
	
	private void RestartScene() {
		if (Input.GetKeyDown (KeyCode.R))
			Application.LoadLevel (Application.loadedLevel);
	}
	
	protected virtual void Teleport() {
        string message = "";
		if (curSceneName == "Castle") {
			if (Input.GetKeyDown (KeyCode.Alpha1)) {
				player.transform.position = new Vector3(-0.007630974f, -1.964107f, -10.39695f);
                message = "Player teleported to starting point";
            } else if (Input.GetKeyDown (KeyCode.Alpha2)) {
                message = "Player teleported to castle foyer";
				player.transform.position = new Vector3(17.18301f, 6.136005f, 72.10401f);
            } else if (Input.GetKeyDown (KeyCode.Alpha3)) {
                message = "Player teleported to castle bridge";
				player.transform.position = new Vector3(16.48485f, 4.944219f, 100.7769f);
            } else if (Input.GetKeyDown (KeyCode.Alpha4)) {
                message = "Player teleported to castle roof";
				player.transform.position = new Vector3(-2.229874f, 34.05241f, 122.4545f);
			}
		} else if (curSceneName == "BobombBattlefield") {
            if (Input.GetKeyDown (KeyCode.Alpha1)) {
                message = "Player teleported to starting point";
				player.transform.position = new Vector3(7.626184f, -2.097889f, -16.86875f);
            } else if (Input.GetKeyDown (KeyCode.Alpha2)) {
                message = "Player teleported to chomp area";
				player.transform.position = new Vector3(-106.8836f, 11.2862f, -86.34922f);
            } else if (Input.GetKeyDown (KeyCode.Alpha3)) {
                message = "Player teleported to mountain entrance";
				player.transform.position = new Vector3(-45.23051f, 11.97652f, -206.2853f);
            } else if (Input.GetKeyDown (KeyCode.Alpha4)) {
                message = "Player teleported to mountain mid-point";
				player.transform.position = new Vector3(-122.3424f, 47.80969f, -175.9443f);
            } else if (Input.GetKeyDown (KeyCode.Alpha5)) {
                message = "Player teleported to mountain summit";
                player.transform.position = new Vector3(-185.179f, 72.37347f, -227.5629f);
            } else if (Input.GetKeyDown (KeyCode.Alpha6)) {
                message = "Player teleported to floating island";
                player.transform.position = new Vector3(-180f, 55f, -105f);
            }
        }
        if (message != "")
            DisplayMessage (message);
    }

    protected void DisplayMessage (string text) {
        Transform existingDebuggerMessage = stereoDialog.transform.Find ("Debugger");
        if (existingDebuggerMessage) {
            Destroy(existingDebuggerMessage.gameObject);
        }
        if (stereoDialog)
            stereoDialog.Create (-450, -650, text, Color.black, TextAlignment.Left, 48, FontStyle.Normal, "Debugger", 5);
        Debug.Log (text);
    }
}
