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
    public GameObject[] menuPanels;

    protected GameObject menu;

    protected void Awake () {
    }

    protected void Start () {
        menu = transform.Find ("Menu").gameObject;
        menu.SetActive (false);
    }
	
    protected void Update () {
        // Keyboard
        if (Input.GetKeyUp(KeyCode.Escape)) {
            menu.SetActive (!menu.activeSelf);
            if (menu.activeSelf) {
                NGUITools.SetActive(initialMenuPanel, true);
            } else {
                // TODO: Refactor inactivating all panels
                foreach (GameObject menuPanel in menuPanels) {
                    NGUITools.SetActive(menuPanel, false);
                }
            }
        }
	}
}
