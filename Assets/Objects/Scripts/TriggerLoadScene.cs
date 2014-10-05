/************************************************************************************

Filename    :   TriggerLoadScene.cs
Content     :   Load specified scene on trigger
Created     :   4 May 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class TriggerLoadScene : MonoBehaviour {
	public string sceneName;

	protected void OnTriggerEnter () {
		if (sceneName != null) {
			Application.LoadLevel(sceneName);
		}
	}
}
