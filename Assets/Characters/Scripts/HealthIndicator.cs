/************************************************************************************

Filename    :   HealthIndicator.cs
Content     :   Displays player's current health on a physical object
Created     :   29 June 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class HealthIndicator : MonoBehaviour {
	public GameObject player;
    public Texture[] healthTextures;			        // Place images of different health increments starting from lowest to highest.
    public Vector3 displayPosition = new Vector3();
    public Vector3 displayPositionNoOVR = new Vector3();
    public Vector3 displayRotation = new Vector3();

    protected PlayerHealth playerHealth;
    protected Vector3 initialPosition;
    protected Quaternion initialRotation;

    private bool HMDPresent;

	protected void Awake () {
        playerHealth = player.GetComponent<PlayerHealth> ();

	}

    protected void Start () {    
#if !UNITY_WEBPLAYER
        HMDPresent = OVRManager.display.isPresent;
#else
        HMDPresent = false;
#endif

        SetPosition ();
    }

	protected void Update () {
        UpdateAction ();    
    }
    
    public void SetPosition () {
        if (StorageManager.data.optionInterfaceDisplayHealth) {
            if (!StorageManager.data.optionControlsEnableRift || !HMDPresent) {
                transform.localPosition = displayPositionNoOVR;
            } else {
                transform.localPosition = displayPosition;
            }
            transform.localRotation = Quaternion.Euler(displayRotation);
            
        } else {
            transform.localPosition = initialPosition;
            transform.localRotation = initialRotation;
        }
    }

    protected void UpdateAction () {
        int health = playerHealth.health;
        Material material = renderer.materials[1];
        
        if (health < 0) {
            material.mainTexture = healthTextures[0];
        } else if (health + 1 > healthTextures.Length) {
            material.mainTexture = healthTextures[healthTextures.Length];
        } else {
            material.mainTexture = healthTextures[health];
        }
    }
}
