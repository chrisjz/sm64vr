/************************************************************************************

Filename    :   MouseLook.cs
Content     :   Player looking around using mouse
Created     :   19 April 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class MouseLook : PlayerLook {

	protected override void Update () {
		axisX = Input.GetAxis("Mouse X");
		axisY = Input.GetAxis("Mouse Y");

		base.Update ();
	}
}
