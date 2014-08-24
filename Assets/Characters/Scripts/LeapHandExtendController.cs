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
    protected Controller leap_controller_;

    protected void Awake () {
        leap_controller_ = new Controller();
    }
	
    protected void LateUpdate () {
        if (leap_controller_ == null)
            return;

        CheckIfHandsEnabled ();
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
