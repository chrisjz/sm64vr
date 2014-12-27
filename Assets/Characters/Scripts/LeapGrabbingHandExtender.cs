/************************************************************************************

Filename    :   LeapHandGrabExtender.cs
Content     :   Extend Leap Grabbing Hand script
Created     :   25 August 2014
Authors     :   Chris Julian Zaharia
Source      :   Based on Leap Motion script GrabbingHand.cs

************************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

public class LeapGrabbingHandExtender : GrabbingHand {
    public bool IsPinched () {
        return (pinch_state_ == PinchState.kPinched) ? true : false;
    }
    public bool IsReleased () {
        return (pinch_state_ == PinchState.kReleased) ? true : false;
    }
    public bool IsReleasing () {
        return (pinch_state_ == PinchState.kReleasing) ? true : false;
    }

    public Collider GetActiveObject () {
        return active_object_;
    }
}
