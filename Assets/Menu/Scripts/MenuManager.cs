using UnityEngine;
using System.Collections;

[AddComponentMenu("NGUI/Interaction/Menu Manager")]
public class MenuManager : MonoBehaviour {
    public enum Type {Action, GoToScene, SwitchPanel}
    public enum Action {Exit}
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
        }
    }
    
    protected void GoToScene() {
        if (nextSceneName != null) {
            Application.LoadLevel(nextSceneName);
        }
    }

    protected void SwitchPanel () {
        NGUITools.SetActive(currentPanelObject, false);
        NGUITools.SetActive(nextPanelObject, true);
    }
}
