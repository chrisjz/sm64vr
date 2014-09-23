/************************************************************************************

Filename    :   LeapHandExtendController.cs
Content     :   Extend Leap Hand Controller
Created     :   25 August 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

public class LeapHandExtendController : HandController {
    public Vector3 GroundedLocalPosition = new Vector3(0, 0, 0);
    public Vector3 GroundedLocalRotation = new Vector3(0, 0, 0);
    public Vector3 MountedLocalPosition = new Vector3(0, 0, 0);
    public Vector3 MountedLocalRotation = new Vector3(0, 0, 0);
    
    protected Controller leap_controller_;
    protected bool isOrientationSet = false;
    protected bool initialised = false;
    
    protected void LateUpdate () {
        if (!initialised) {
            Initialise ();
        }

        if (leap_controller_ == null)
            return;

        SetOrientation ();
        CheckIfHandsEnabled ();
    }

    protected void Initialise () {
        leap_controller_ = new Controller ();
        initialised = true;
    }
    
    // Set position of hands based on if leap is grounded/mounted
    protected void SetOrientation () {
        if (!isOrientationSet) {
            if (isHeadMounted) {
                transform.localEulerAngles  = GroundedLocalRotation;
                transform.localPosition = MountedLocalPosition;
            } else {
                transform.localEulerAngles = GroundedLocalRotation;
                transform.localPosition = GroundedLocalPosition;
            }

            isOrientationSet = true;
        }
    }
    
    protected void CheckIfHandsEnabled () {
        Frame frame = leap_controller_.Frame();
        HandList hands = frame.Hands;
        int num_hands = hands.Count;
        
        if (num_hands > 0) {
            TriggerSixenseHands (false);
        } else {
            TriggerSixenseHands (true);
        }
    }
    
    protected void TriggerSixenseHands (bool state) {
        Transform player = transform.root;
        player.Find ("Avatar/Left_Hand").renderer.enabled = state;
        player.Find ("Avatar/Left_Upper_Arm").renderer.enabled = state;
        player.Find ("Avatar/Left_Lower_Arm").renderer.enabled = state;
        player.Find ("Avatar/Right_Hand").renderer.enabled = state;
        player.Find ("Avatar/Right_Upper_Arm").renderer.enabled = state;
        player.Find ("Avatar/Right_Lower_Arm").renderer.enabled = state;
    }
}
