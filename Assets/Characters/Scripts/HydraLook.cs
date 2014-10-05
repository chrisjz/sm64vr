/************************************************************************************

Filename    :   HydraLook.cs
Content     :   Player looking around using Razer Hydra
Created     :   18 May 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class HydraLook : PlayerLook {
	
	protected override void Update () {
		// Get the input vector from hydra
		SixenseInput.Controller hydraRightController = SixenseInput.GetController (SixenseHands.RIGHT);
		
		if (hydraRightController != null) {
			axisX = hydraRightController.JoystickX;
			axisY = hydraRightController.JoystickY;
		}
		
		base.Update ();
	}
}
