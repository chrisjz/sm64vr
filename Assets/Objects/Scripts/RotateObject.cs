/************************************************************************************

Filename    :   RotateObject.cs
Content     :   Rotate object by direction and speed
Created     :   4 May 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {
	public float speed;
    public Direction direction = Direction.Z;

    public enum Direction{X, Y, Z};

	void Update () {
        if (direction == Direction.X) {
            transform.Rotate(Time.deltaTime * speed, 0, 0);
        } else if (direction == Direction.Y) {
            transform.Rotate(0, Time.deltaTime * speed, 0);
        } else if (direction == Direction.Z) {
            transform.Rotate(0, 0, Time.deltaTime * speed);
        }
	}
}
