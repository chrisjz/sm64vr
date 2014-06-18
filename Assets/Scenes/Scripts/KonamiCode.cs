/************************************************************************************

Filename    :   KonamiCode.cs
Content     :   Konami code to trigger a specific action
Created     :   18 June 2014
Authors     :   Chris Julian Zaharia

Credit to "save" from Unity Answers for the original JS code.

************************************************************************************/

using UnityEngine;
using System.Collections;

public class KonamiCode : MonoBehaviour {
	public bool enable = false;

	private string[] konamiCode = new string[]{"UpArrow", "UpArrow", "DownArrow", "DownArrow", "LeftArrow", "RightArrow", "LeftArrow", "RightArrow", "B", "A", "Return"};
	private int currentPos = 0;
	private bool konamiCodeEnabled = false;

	void OnGUI () {
		if (!enable) {
			return;
		}

		Event e = Event.current;

		if (e.isKey && Input.anyKeyDown && !konamiCodeEnabled && e.keyCode.ToString() != "None") {
			KonamiFunction (e.keyCode);
		}
	}

	protected void KonamiFunction (KeyCode incomingKey) {
		string incomingKeyString = incomingKey.ToString ();
		if (incomingKeyString == konamiCode[currentPos]) {
			//Debug.Log("Unlocked part "+(currentPos+1)+"/"+konamiCode.Length+" with "+incomingKeyString);
			currentPos++;

			if ((currentPos + 1) > konamiCode.Length) {
				//Debug.Log("You master Konami.");
				SetKonamiCodeEnabled(true);
				currentPos = 0;
			}
		} else {
			//Debug.Log("You fail Konami at position "+(currentPos+1)+", find the ninja in you.");
			currentPos=0;
		}
	}

	public bool IsKonamiCodeEnabled () {
		return konamiCodeEnabled;
	}

	public void SetKonamiCodeEnabled (bool state) {
		konamiCodeEnabled = state;
	}
}
