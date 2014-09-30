/************************************************************************************

Filename    :   CoinIndicator.cs
Content     :   Displays player's current coins on a physical object
Created     :   1 October 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class CoinIndicator : MonoBehaviour {
    public Vector3 displayPosition = new Vector3();
    public Vector3 displayPositionNoOVR = new Vector3();
    public Vector3 displayRotation = new Vector3();
    public Vector3 displayRotationNoOVR = new Vector3();
    
    protected GameObject player;
    protected GameObject coinIndicator;
    protected SceneManager sceneManager;
    protected Vector3 initialPosition;
    protected Quaternion initialRotation;
    
    private bool HMDPresent;
    
    protected void Awake () {
        player = GameObject.FindGameObjectWithTag ("Player");
        coinIndicator = transform.Find ("Count").gameObject;
        sceneManager = GameObject.FindObjectOfType<SceneManager>();
        
    }
    
    protected void Start () {    
        HMDPresent = OVRDevice.IsHMDPresent();
        
        SetPosition ();
    }
    
    protected void Update () {
        UpdateAction ();   
    }
    
    public void SetPosition () {
        if (StorageManager.data.optionInterfaceDisplayCoins && Application.loadedLevelName != "Castle") {
            SetVisibility(true);
            if (!StorageManager.data.optionControlsEnableRift || !HMDPresent) {
                transform.localPosition = displayPositionNoOVR;
                transform.localRotation = Quaternion.Euler(displayRotationNoOVR);
            } else {
                transform.localPosition = displayPosition;
                transform.localRotation = Quaternion.Euler(displayRotation);
            }
            
        } else {
            SetVisibility(false);
        }
    }
    
    public void UpdateAction () {
        if (!sceneManager)
            return;
        
        if (coinIndicator) {
            TextMesh coinText = coinIndicator.GetComponent<TextMesh> ();
            coinText.text = sceneManager.coins.ToString ();
        }
    }
    
    protected void SetVisibility(bool state) {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
            renderer.enabled = state;
        }
        
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders) {
            col.enabled = state;
        }
    }
}
