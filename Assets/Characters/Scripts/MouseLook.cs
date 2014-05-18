using UnityEngine;
using System.Collections;

public class MouseLook : PlayerLook {

	protected override void Update () {
		axisX = Input.GetAxis("Mouse X");
		axisY = Input.GetAxis("Mouse Y");

		base.Update ();
	}
}
