/************************************************************************************

Filename    :   MenuManager.cs
Content     :   Manage NGUI menu actions
Created     :   17 September 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

[AddComponentMenu("NGUI/Interaction/Menu Manager")]
public class MenuManager : MonoBehaviour {
    public enum Type {Action, GoToScene, SwitchPanel}
    public enum Action {Exit, Resume, Restart}
    public Type type;
    public Action action;
    public string nextSceneName;
    public GameObject currentPanelObject;
    public GameObject nextPanelObject;
    
    void OnClick() {
        switch (type) {
        case Type.Action:
            UpdateAction ();
            break;
        case Type.GoToScene:
            GoToScene ();
            break;
        case Type.SwitchPanel:
            SwitchPanel();
            break;
        }
    }
    
    protected void UpdateAction() {
        switch (action) {
        case Action.Exit:
            Application.Quit();
            break;
        case Action.Resume:
            Resume();
            break;
        case Action.Restart:
            Application.LoadLevel (Application.loadedLevel);
            break;
        }
    }

    protected void Resume() {
        Transform player = GameObject.FindGameObjectWithTag ("Player").transform;
        GameObject menu = player.Find ("Menu").gameObject;
        FPSInputController inputController = player.GetComponent<FPSInputController> ();
        menu.SetActive (false);
        inputController.enableMovement = true;
    }
    
    protected void GoToScene() {
        if (nextSceneName != null) {
            InitNextScene ();
            Application.LoadLevel(nextSceneName);
        }
    }

    protected void SwitchPanel () {
        NGUITools.SetActive(currentPanelObject, false);
        NGUITools.SetActive(nextPanelObject, true);
    }

    // Pass through any required cross scene variables
    protected virtual void InitNextScene () {
        string currentSceneName = Application.loadedLevelName;

        if (currentSceneName == "BobombBattlefield") {
            SceneManager sceneManager = (SceneManager) FindObjectOfType(typeof(SceneManager));

            if (sceneManager)
                sceneManager.SaveScore ();

            PlayerPrefs.SetString ("previousSceneName", currentSceneName);
            PlayerPrefs.SetString ("previousSceneExitAction", "exit");
        }
    }
}
