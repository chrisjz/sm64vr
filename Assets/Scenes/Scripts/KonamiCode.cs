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
    public bool disableIfReentered = false;     // Disables Konami code if re-entered.

	private string[] konamiCode = new string[]{"UpArrow", "UpArrow", "DownArrow", "DownArrow", "LeftArrow", "RightArrow", "LeftArrow", "RightArrow", "B", "A", "Return"};
	private int currentPos = 0;
	private bool konamiCodeEnabled = false;

	void OnGUI () {
		if (!enable)
			return;

		Event e = Event.current;

        if (e.isKey && Input.anyKeyDown && (disableIfReentered || (!konamiCodeEnabled && !disableIfReentered))  &&
                e.keyCode.ToString() != "None")
			KonamiFunction (e.keyCode);
	}

	protected void KonamiFunction (KeyCode incomingKey) {
		string incomingKeyString = incomingKey.ToString ();
		if (incomingKeyString == konamiCode[currentPos]) {
			//Debug.Log("Unlocked part "+(currentPos+1)+"/"+konamiCode.Length+" with "+incomingKeyString);
			currentPos++;

			if ((currentPos + 1) > konamiCode.Length) {
                if (disableIfReentered && konamiCodeEnabled) {
                    Debug.Log("Konami code disabled.");
                    konamiCodeEnabled = false;
                } else {
                    Debug.Log("You master Konami.");
                    konamiCodeEnabled = true;
                }

				currentPos = 0;
			}
		} else {
			//Debug.Log("You fail Konami at position "+(currentPos+1)+", find the ninja in you.");
			currentPos=0;
		}
	}

    public bool KonamiCodeEnabled {
        get {
            return konamiCodeEnabled;
        }
        set {
            konamiCodeEnabled = value;
        }
    }
}
