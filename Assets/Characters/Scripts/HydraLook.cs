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
