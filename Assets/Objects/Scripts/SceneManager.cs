/************************************************************************************

Filename    :   SceneManager.cs
Content     :   Manage current scene specific identifiers and cross scene functions
Created     :   16 August 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {
    public int id;                              // Unique ID for world scenes, used to store coin scores.
    public AudioClip sceneAudioClip;            // Main music played throughout scene.
	public string menuSceneDescriptionText;
    public string exitSceneName;                // Go to this scene when exiting level.
    public int coins = 0;                       // coin counter.
    public int redCoins = 0;                    // red coin counter.

    protected GameObject player;

    protected virtual void Start () {
        player = GameObject.FindGameObjectWithTag ("Player");
#if UNITY_WEBPLAYER
        SetupWebPlayer();
#endif
        Spawn ();
    }

    // Setup the scene for web player
    protected virtual void SetupWebPlayer () {
        if (!player)
            return;

        SixenseInput sixenseInput = player.GetComponentInChildren<SixenseInput> ();
        
        if (sixenseInput)
            sixenseInput.enabled = false;

        SixenseHandControllerExtender[] sixenseHandControllers = player.GetComponentsInChildren<SixenseHandControllerExtender> ();

        foreach (SixenseHandControllerExtender sixenseHandController in sixenseHandControllers)
            sixenseHandController.enabled = false;

        LeapHandControllerExtender leapHandController = player.GetComponentInChildren<LeapHandControllerExtender> ();

        if (leapHandController)
            leapHandController.enabled = false;
    }

    // Set spawn if requested by previous scene
    protected virtual void Spawn () {
        if (!player)
            return;

        string currentSceneName = Application.loadedLevelName;
        string previousSceneName = PlayerPrefs.GetString ("previousSceneName");
        string previousSceneExitAction = PlayerPrefs.GetString ("previousSceneExitAction");

        if (currentSceneName == "Castle" && previousSceneName == "BobombBattlefield") {
            if (previousSceneExitAction != null) {
                GameObject enterSectionsEntrance = GameObject.Find ("Enter Sections Entrance");
                TriggerActiveSections triggerEntranceSections = enterSectionsEntrance.GetComponent<TriggerActiveSections> ();
                triggerEntranceSections.TriggerSection ();
            }
            if (previousSceneExitAction == "death") {
                GameObject exitSectionsBobombRoom = GameObject.Find ("Exit Sections Bobomb Room");
                TriggerActiveSections triggerBobombRoomSections = exitSectionsBobombRoom.GetComponent<TriggerActiveSections> ();
                triggerBobombRoomSections.TriggerSection ();
                player.transform.position = new Vector3 (-32.42915f, 15f, 126.7374f);
                player.transform.rotation = Quaternion.Euler (new Vector3 (0, -90f, 0));
            } else if (previousSceneExitAction == "exit") {
                player.transform.position = new Vector3 (16.13499f, 4.349323f, 112.5232f);
                player.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
            }
        }

        PlayerPrefs.SetString ("previousSceneName", null);
        PlayerPrefs.SetString ("previousSceneExitAction", null);
    }

    public void SaveScore () {
        int[] scoreCoins = new int[14];
        if (StorageManager.data.coins != null)
            scoreCoins = StorageManager.data.coins;
        else
            StorageManager.data.coins = new int[14];

        if (id < scoreCoins.Length && coins > scoreCoins[id]) {
            StorageManager.data.coins[id] = coins;
            StorageManager.Save ();
        }
    }
}
