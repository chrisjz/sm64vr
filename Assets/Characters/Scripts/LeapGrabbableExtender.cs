/************************************************************************************

Filename    :   LeapGrabbableExtender.cs
Content     :   Extends Leap Motion's Grabbable
Created     :   2 October 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class LeapGrabbableExtender : GrabbableObject {
    public bool canGrab = true;
    public float overrideGrabDistance = 0f;          // Will override the grab distance defined in the hand's grab hand script, if set higher than 0;
}
