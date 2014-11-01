/************************************************************************************

Filename    :   FPSInputController.cs
Content     :   Rewrite and extension of Unity's FPSInputController javascript
Created     :   19 April 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

// Require a character controller to be attached to the same game object
[RequireComponent(typeof(CharacterMotor))]
[AddComponentMenu("Character/FPS Input Controller")]

public class FPSInputController : MonoBehaviour {
	public enum OvrCameras { Left, Right }
	public OvrCameras mainOvrCamera = OvrCameras.Right;                 // OVR Camera where movement is oriented towards
    public AudioClip[] initialJumpAudioClips;
    public bool detectOvr = true;                                       // Detects if player is using Oculus Rift
    public Vector3 ovrControlSensitivity = new Vector3(1, 1, 1);        // Multiplier of positiona tracking move/jump actions
    public Vector3 ovrControlMinimum = new Vector3(0, 0, 0);            // Min distance of head from centre to move/jump
    public enum OvrXAxisAction { Strafe = 0, Rotate = 1 }
    public OvrXAxisAction ovrXAxisAction = OvrXAxisAction.Rotate;       // Whether x axis positional tracking performs straffing or rotation
    public float ovrHydraLookSensitivityX = 0;
    public float ovrHydraLookSensitivityY = 0;
    public bool enableMovement;

    // Fall damage
    public float fallDamageHeight;              					    // Player receives X damage per multiplication of this height
    public int fallDamageHealth;                                        // Amount of health player loses per fallen height increment

    protected PlayerHealth playerHealth;

	private CharacterMotor motor;
    private OVRCameraRig ovrCameraRig;
    private OVRManager ovrManager;
    private GameObject ovrCameraLeft;
    private GameObject ovrCameraRight;
    private GameObject generalCamera;                                   // Camera for monoscopic view
    private GameObject dirOvrCamera;                                    // Movement oriented using this camera for OVR
    private GameObject mainCamera;                                      // Camera where movement orientation is done
    private Vector3 directionVector;
	private float defaultMaxForwardSpeed;
    private float defaultMaxBackwardsSpeed;
    private float initialVerticalPosition;
    private float previousVerticalPosition;
    private float finalVerticalPosition;
	private bool inputEnabled;					                        // If input is enabled/disabled
    private bool jumpEnabled;
	private bool HMDPresent = false;

    // OVR positional tracking, currently works via tilting head
    private Vector3 initPosTrackDir;
    private Vector3 curPosTrackDir;
    private Vector3 diffPosTrackDir;

    protected HydraLook[] hydraLookObjects;
	
	// Use this for initialization
	void  Awake (){
        motor = GetComponent<CharacterMotor>();
        playerHealth = gameObject.GetComponent<PlayerHealth> ();
        HMDPresent = OVRManager.display.isPresent;
        enableMovement = true;
		defaultMaxForwardSpeed = motor.movement.maxForwardSpeed;
        defaultMaxForwardSpeed = motor.movement.maxBackwardsSpeed;
        hydraLookObjects = gameObject.GetComponentsInChildren<HydraLook> ();

        // Camera rig
        ovrCameraRig = gameObject.GetComponentInChildren<OVRCameraRig> ();
        ovrManager = gameObject.GetComponentInChildren<OVRManager> ();

        // Cameras
        ovrCameraLeft = transform.Find("OVRCameraRig/LeftEyeAnchor").gameObject;
        ovrCameraRight = transform.Find("OVRCameraRig/RightEyeAnchor").gameObject;
        generalCamera = transform.Find("OVRCameraRig/MonoEyeAnchor").gameObject;
	}

	void Start() {
		IgnorePlayerColliders ();
        InitCamera();
        previousVerticalPosition = initialVerticalPosition = transform.position.y;
        initPosTrackDir = mainCamera.transform.localPosition;
		inputEnabled = true;
        jumpEnabled = true;
	}
	
	// Update is called once per frame
	void  Update (){
		if (!inputEnabled) {
			return;
		}

        UpdateKeyInput ();

        if (enableMovement) {
            UpdateMovement ();
        } else {
            motor.inputMoveDirection = new Vector3(0, 0, 0);
        }

		UpdateAnimation ();
	}

    void FixedUpdate () {
        float verticalMovement = VerticalMovement ();
        if (verticalMovement < 0) {
            HandleFallDamage(verticalMovement);
        }
    }

    protected void UpdateMovement () {
        // Get the input vector from keyboard or analog stick
        directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        
        // Get the input vector from OVR positional tracking
        if (StorageManager.data.optionControlsRiftPosTrackMove || StorageManager.data.optionControlsRiftPosTrackJump) {
            curPosTrackDir = mainCamera.transform.localPosition;
            diffPosTrackDir = curPosTrackDir - initPosTrackDir;
        }
        
        if (StorageManager.data.optionControlsRiftPosTrackMove && HMDPresent) {
            if (diffPosTrackDir.x <= -ovrControlMinimum.x || diffPosTrackDir.x >= ovrControlMinimum.x) {
                if (ovrXAxisAction == OvrXAxisAction.Strafe) {
                    diffPosTrackDir.x *= ovrControlSensitivity.x;
                } else {
                    transform.Rotate(0, diffPosTrackDir.x * ovrControlSensitivity.x, 0);
                    diffPosTrackDir.x = 0;
                }
            } else {
                diffPosTrackDir.x = 0;
            }
            
            if (diffPosTrackDir.z <= -ovrControlMinimum.z || diffPosTrackDir.z >= ovrControlMinimum.z) {
                diffPosTrackDir.z *= ovrControlSensitivity.z;
            } else {
                diffPosTrackDir.z = 0;
            }
            
            directionVector = new Vector3(diffPosTrackDir.x, 0, diffPosTrackDir.z);
        }
        
        // Get the input vector from hydra
        SixenseInput.Controller hydraLeftController = SixenseInput.GetController (SixenseHands.LEFT);
        SixenseInput.Controller hydraRightController = SixenseInput.GetController (SixenseHands.RIGHT);
        
        if (hydraLeftController != null) {
            directionVector= new Vector3(hydraLeftController.JoystickX, 0, hydraLeftController.JoystickY);
        }
        
        if (jumpEnabled) {
            if (hydraRightController != null) {
                motor.inputJump = hydraRightController.GetButton (SixenseButtons.BUMPER);
            } else if (StorageManager.data.optionControlsRiftPosTrackJump && HMDPresent) {
                motor.inputJump = GetPositionalTrackingYForJump();
            } else {
                motor.inputJump = Input.GetButton ("Jump");
            }
        }
        
        // Play jumping audio clips
        if (initialJumpAudioClips.Length > 0 && motor.inputJump && motor.grounded && !audio.isPlaying) {
            audio.clip = initialJumpAudioClips[Random.Range(0, initialJumpAudioClips.Length)];
            audio.Play();
        }
        
        if (directionVector != Vector3.zero) {
            // Get the length of the directon vector and then normalize it
            // Dividing by the length is cheaper than normalizing when we already have the length anyway
            float directionLength= directionVector.magnitude;
            directionVector = directionVector / directionLength;
            
            // Make sure the length is no bigger than 1
            directionLength = Mathf.Min(1, directionLength);
            
            // Make the input vector more sensitive towards the extremes and less sensitive in the middle
            // This makes it easier to control slow speeds when using analog sticks
            directionLength = directionLength * directionLength;
            
            // Multiply the normalized direction vector by the modified length
            directionVector = directionVector * directionLength;
        }       

        motor.inputMoveDirection = mainCamera.transform.rotation * directionVector;
    }

    protected void UpdateKeyInput() {
        // Trigger movement via OVR positional tracking
        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) &&
                Input.GetKeyDown(KeyCode.M)) {
            StorageManager.data.optionControlsRiftPosTrackMove = !StorageManager.data.optionControlsRiftPosTrackMove;
        }
        
        // Trigger jump via OVR positional tracking
        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) &&
                Input.GetKeyDown(KeyCode.J)) {
            StorageManager.data.optionControlsRiftPosTrackJump = !StorageManager.data.optionControlsRiftPosTrackJump;
        }
        
        // Trigger between straffing and rotating for X axis on movement via OVR positional tracking
        if ((Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) &&
                Input.GetKeyDown(KeyCode.M)) {
            if (ovrXAxisAction == OvrXAxisAction.Strafe) {
                ovrXAxisAction = OvrXAxisAction.Rotate;
            } else {
                ovrXAxisAction = OvrXAxisAction.Strafe;
            }
        }

    }

	void UpdateAnimation() {
        if (animation && directionVector.z != 0) {
			animation["Walk"].speed = Mathf.Abs(directionVector.z);
			animation.CrossFade("Walk");
		}
	}

    public void InitCamera() {
        if (mainOvrCamera == OvrCameras.Left) {
            dirOvrCamera = ovrCameraLeft;
        } else {
            dirOvrCamera = ovrCameraRight;
        }
        
        // Apply the direction to the CharacterMotor
        if (!StorageManager.data.optionControlsEnableRift || (detectOvr && !HMDPresent)) {
            mainCamera = generalCamera;
        } else {
            mainCamera = dirOvrCamera;
        }
        
        if (detectOvr) {
            DetectOVR();
        }
    }

    protected bool GetPositionalTrackingYForJump() {
        if (diffPosTrackDir.y > ovrControlMinimum.y) {
            return true;
        } else {
            return false;
        }
    }

	public void SetInputEnabled (bool status) {
		inputEnabled = status;
	}

    public bool JumpEnabled {
        get {
            return jumpEnabled;
        }
        set {
            jumpEnabled = value;
        }
    }

    // Track player's vertical movement to handle fall damage.
    protected float VerticalMovement () {
        if (transform.position.y != previousVerticalPosition) {
            previousVerticalPosition = transform.position.y;
            return 0;
        } else {
            finalVerticalPosition = transform.position.y;
            float totalVerticalDistance = Mathf.Round((finalVerticalPosition - initialVerticalPosition)*1f)/1f;
            initialVerticalPosition = transform.position.y;

            return totalVerticalDistance;
        }
    }

    protected void HandleFallDamage (float distance) {
        if (distance < -fallDamageHeight) {
            int fallHeightIncrement = Mathf.FloorToInt(-distance / fallDamageHeight);
            playerHealth.Damage(fallDamageHealth * fallHeightIncrement);
        }
    }

	// Show OVR Camera only if OVR is being used
	protected void DetectOVR() {
        if (!HMDPresent || !StorageManager.data.optionControlsEnableRift) {
            ovrCameraLeft.SetActive(false);
            ovrCameraRight.SetActive(false);
            generalCamera.SetActive(true);
            ovrCameraRig.enabled = false;
            ovrManager.enabled = false;
		} else {
            ovrCameraLeft.SetActive(true);
            ovrCameraRight.SetActive(true);
            generalCamera.SetActive(false);
            ovrCameraRig.enabled = true;
            ovrManager.enabled = true;
            SetHydraLookSensitivity ();
		}
	}

	// Prevent colliders on player from colliding with each other i.e. hand colliders with body collider
	void IgnorePlayerColliders () {
		Collider[] cols = GetComponentsInChildren<Collider>();

		foreach (Collider col in cols) {
			if (col != collider) {
				Physics.IgnoreCollision(col, collider);
			}
		}
	}
	
	public float getDefaultMaxForwardSpeed () {
		return defaultMaxForwardSpeed;
	}
	
	public float getDefaultMaxBackwardsSpeed () {
		return defaultMaxForwardSpeed;
    }
    
    protected void SetHydraLookSensitivity () {
        foreach (HydraLook obj in hydraLookObjects) {
            obj.sensitivityX = ovrHydraLookSensitivityX;
            obj.sensitivityY = ovrHydraLookSensitivityY;
        }
    }
}