using UnityEngine;
using System.Collections;

[AddComponentMenu("NGUI/Interaction/Menu Manager")]
public class MenuManager : MonoBehaviour {
    public enum Type {Action, Switcher}
    public enum Action {Exit}
    public Type type;
    public Action action;
    public GameObject currentPanelObject;
    public GameObject nextPanelObject;
    
    void OnClick() {
        switch (type) {
            case Type.Action:
                UpdateAction ();
                break;
            case Type.Switcher:
                Switch();
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

    protected void Switch () {
        NGUITools.SetActive(currentPanelObject, false);
        NGUITools.SetActive(nextPanelObject, true);
    }
}
