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
	public OvrCameras mainOvrCamera = OvrCameras.Right;                 // OVR Camera where movement is oriented towards, should have audio listener too
    public AudioClip[] initialJumpAudioClips;
    public bool enableMoveViaTiltOvr = false;                           // Move player by moving head
    public float minDistMoveViaOvr = 0;                             // Move using head positional tracking if position from body at least this far on X and Z axis.
    public bool enableJumpViaOvr = false;                               // Player jumps by moving head high up
    public float minDistJumpViaOvr = 0;                             // Jump using head positional tracking if position from body at least this far on Y axis.
	public bool enableDetectOvr = true;								    // Detects if player is using Oculus Rift

    // Fall damage
    public float fallDamageHeight;              					    // Player receives X damage per multiplication of this height
    public int fallDamageHealth;                                        // Amount of health player loses per fallen height increment

    protected PlayerHealth playerHealth;

	private CharacterMotor motor;
    private GameObject ovrCameraLeft;
    private GameObject ovrCameraRight;
    private GameObject generalCamera;                                   // Camera for monoscopic view
    private GameObject dirOvrCamera;                                    // Movement oriented using this camera for OVR
    private GameObject mainCamera;                                      // Camera where movement orientation is done and audio listener enabled
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
	
	// Use this for initialization
	void  Awake (){
        motor = GetComponent<CharacterMotor>();
        playerHealth = gameObject.GetComponent<PlayerHealth> ();
		defaultMaxForwardSpeed = motor.movement.maxForwardSpeed;
		defaultMaxForwardSpeed = motor.movement.maxBackwardsSpeed;

        // Cameras
        ovrCameraLeft = transform.Find("OVRCameraController/CameraLeft").gameObject;
        ovrCameraRight = transform.Find("OVRCameraController/CameraRight").gameObject;
        generalCamera = transform.Find("OVRCameraController/Camera").gameObject;
	}

	void Start() {
		IgnorePlayerColliders ();
        InitCamera();
        previousVerticalPosition = initialVerticalPosition = transform.position.y;
        initPosTrackDir = mainCamera.transform.InverseTransformDirection(transform.position - mainCamera.transform.position);
		inputEnabled = true;
        jumpEnabled = true;
	}
	
	// Update is called once per frame
	void  Update (){
		if (!inputEnabled) {
			return;
		}

		// Get the input vector from keyboard or analog stick
		Vector3 directionVector= new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Get the input vector from OVR positional tracking via tilting
        // TODO: Sideways movement will rotate player instead of straff
        // TODO: also get input vector via head position
        if (enableMoveViaTiltOvr || enableJumpViaOvr) {
            curPosTrackDir = mainCamera.transform.InverseTransformDirection(transform.position - mainCamera.transform.position);
        }

        if (enableMoveViaTiltOvr) {
            diffPosTrackDir = curPosTrackDir - initPosTrackDir;
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
            } else{
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

		UpdateAnimations (directionVector);
	}

    void FixedUpdate () {
        float verticalMovement = VerticalMovement ();
        if (verticalMovement < 0) {
            HandleFallDamage(verticalMovement);
        }
    }

	void UpdateAnimations(Vector3 directionVector) {
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
        HMDPresent = OVRDevice.IsHMDPresent();
        if (enableDetectOvr && !HMDPresent) {
            mainCamera = generalCamera;
        } else {
            mainCamera = dirOvrCamera;
        }
        
        if (enableDetectOvr) {
            DetectOVR();
        }
    }

    // TODO: Bind to jump action
    protected bool GetPositionalTrackingYForJump() {
        if (initPosTrackDir.y - curPosTrackDir.y > minDistJumpViaOvr) {
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
		HMDPresent = OVRDevice.IsHMDPresent();

		if (HMDPresent == false) {
            ovrCameraLeft.SetActive(false);
            ovrCameraRight.SetActive(false);
            generalCamera.SetActive(true);
            
            generalCamera.GetComponent<AudioListener>().enabled = true;
            dirOvrCamera.GetComponent<AudioListener>().enabled = false;
		} else {
            ovrCameraLeft.SetActive(true);
            ovrCameraRight.SetActive(true);
            generalCamera.SetActive(false);
            
            generalCamera.GetComponent<AudioListener>().enabled = false;
            dirOvrCamera.GetComponent<AudioListener>().enabled = true;
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
}