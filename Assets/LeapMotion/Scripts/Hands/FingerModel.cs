﻿/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// Interface for all fingers.
public abstract class FingerModel : MonoBehaviour {

  public const int NUM_BONES = 4;
  public const int NUM_JOINTS = 5;

  public Finger.FingerType fingerType = Finger.FingerType.TYPE_INDEX;

  private Hand hand_;
  private Finger finger_;
  private Vector3 offset_ = Vector3.zero;

  HandController controller_;

  public void SetController(HandController controller) {
    controller_ = controller;
  }

  public HandController GetController() {
    return controller_;
  }

  public void SetLeapHand(Hand hand) {
    hand_ = hand;
    finger_ = hand.Fingers[(int)fingerType];
  }

  public void SetOffset(Vector3 offset) {
    offset_ = offset;
  }

  public Hand GetLeapHand() { return hand_; }
  public Finger GetLeapFinger() { return finger_; }

  public abstract void InitFinger();

  public abstract void UpdateFinger();

  // Returns any additional movement the finger needs because of non-relative palm movement.
  public Vector3 GetOffset() {
    return offset_;
  }

  // Returns the location of the tip of the finger in relation to the controller.
  public Vector3 GetTipPosition() {
    Vector3 local_tip = finger_.Bone((Bone.BoneType.TYPE_DISTAL)).NextJoint.ToUnityScaled();
    return controller_.transform.TransformPoint(local_tip) + offset_;
  }

  // Returns the location of the given joint on the finger in relation to the controller.
  public Vector3 GetJointPosition(int joint) {
    if (joint >= NUM_BONES)
      return GetTipPosition();
    
    Vector3 local_position = finger_.Bone((Bone.BoneType)(joint)).PrevJoint.ToUnityScaled();
    return controller_.transform.TransformPoint(local_position) + offset_;
  }

  // Returns a ray from the tip of the finger in the direction it is pointing.
  public Ray GetRay() {
    Ray ray = new Ray(GetTipPosition(), GetBoneDirection(NUM_BONES - 1));
    return ray;
  }

  // Returns the center of the given bone on the finger in relation to the controller.
  public Vector3 GetBoneCenter(int bone_type) {
    Bone bone = finger_.Bone((Bone.BoneType)(bone_type));
    return controller_.transform.TransformPoint(bone.Center.ToUnityScaled()) + offset_;
  }

  // Returns the direction the given bone is facing on the finger in relation to the controller.
  public Vector3 GetBoneDirection(int bone_type) {
    Vector3 direction = GetJointPosition(bone_type + 1) - GetJointPosition(bone_type);
    return direction.normalized;
  }

  // Returns the rotation quaternion of the given bone in relation to the controller.
  public Quaternion GetBoneRotation(int bone_type) {
    Quaternion local_rotation = finger_.Bone((Bone.BoneType)(bone_type)).Basis.Rotation();
    return controller_.transform.rotation * local_rotation;
  }
}
