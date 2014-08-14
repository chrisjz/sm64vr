/************************************************************************************

Filename    :   CubeMenuButton.cs
Content     :   Buttons for 3D cube menu
Created     :   11 August 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class CubeMenuButton : MonoBehaviour {
	public string buttonName;
	public bool buttonEnabled;

	protected CubeMenu menu;
	
	protected virtual void Awake() {
		menu = transform.parent.parent.parent.GetComponent<CubeMenu> ();
	}

	void OnTriggerEnter(Collider other) {
		if (!buttonEnabled) {
			return;
		}

		menu.HandleButtonByName(buttonName);
	}
}
