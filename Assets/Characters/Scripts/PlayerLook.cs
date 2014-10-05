/************************************************************************************

Filename    :   PlayerLook.cs
Content     :   PlayerLook rotates the transform based on the input device's delta.
Created     :   18 May 2014
Authors     :   Chris Julian Zaharia

Minimum and Maximum values can be used to constrain the possible rotation.

Based on Unity's MouseLook script.
************************************************************************************/

using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Player Look")]
public class PlayerLook : MonoBehaviour {
	
	public enum RotationAxes { XAndY = 0, X = 1, Y = 2 }
	public RotationAxes axes = RotationAxes.XAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	
	protected float rotationY = 0F;

	protected float axisX, axisY;

	private float defaultSensitivityX, defaultSensitivityY;

	protected virtual void Awake () {
		defaultSensitivityX = sensitivityX;
		defaultSensitivityY = sensitivityY;
	}

	protected virtual void Start () {
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}

	protected virtual void Update () {
		
		if (axes == RotationAxes.XAndY)
		{
			float rotationX = transform.localEulerAngles.y + axisX * sensitivityX;
			
			rotationY += axisY * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.X)
		{
			transform.Rotate(0, axisX * sensitivityX, 0);
		}
		else
		{
			rotationY += axisY * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}
	}
	
	public float getDefaultSensitivityX () {
		return defaultSensitivityX;
	}
	
	public float getDefaultSensitivityY () {
		return defaultSensitivityY;
	}
}
