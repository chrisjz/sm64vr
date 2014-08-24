﻿/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// The finger model for our skeletal hand made out of various non-deforming models.
public class SkeletalFinger : FingerModel {

  public Transform[] bones = new Transform[NUM_BONES];

  public override void InitFinger() {
    SetPositions();
  }

  public override void UpdateFinger() {
    SetPositions();
  }

  private void SetPositions() {
    for (int i = 0; i < bones.Length; ++i) {
      if (bones[i] != null) {
        bones[i].transform.position = GetBoneCenter(i);
        bones[i].transform.rotation = GetBoneRotation(i);
      }
    }
  }
}
