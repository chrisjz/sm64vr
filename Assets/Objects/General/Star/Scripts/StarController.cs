/************************************************************************************

Filename    :   StarController.cs
Content     :   Controller for star object
Created     :   23 August 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class StarController : MonoBehaviour {
    public string worldSceneName = "Castle";                        // Scene where user goes to after grabbing star
    public AudioClip spawnAudioClip;
    public AudioClip captureAudioClip;
    public Vector3 finalPointOffset = new Vector3 (0, 1.5f, 0);
    public float speed = 3f;

    protected GameObject worldTheme;
    protected string defaultTag;
    protected bool starMovingToFinalPoint;
    protected bool grabbedStar;

    protected virtual void Awake () {
        worldTheme = GameObject.Find ("Theme");
        starMovingToFinalPoint = false;
        grabbedStar = false;
        defaultTag = tag;
        tag = "Untagged";
    }

    protected virtual void Update () {
        if (starMovingToFinalPoint) {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, transform.position + finalPointOffset, step);
        }
    }
    
    protected void OnCollisionEnter(Collision col) {
        Debug.Log (col.gameObject.name);
        if (col.gameObject.name == "RightHandCollider" || col.gameObject.name == "LeftHandCollider") {
            if (!grabbedStar) {
                StartCoroutine(GrabStar ());
            }
            grabbedStar = true;
        }
    }

    public void Spawn () {
        StartCoroutine(PlaySpawnAudio ());
        StartCoroutine (MoveToFinalPoint ());
    }
    
    protected IEnumerator GrabStar() {
        worldTheme.audio.clip = captureAudioClip;
        worldTheme.audio.Play ();
        yield return new WaitForSeconds(captureAudioClip.length);
        Application.LoadLevel(worldSceneName);
        
    }

    protected IEnumerator PlaySpawnAudio () {
        audio.clip = spawnAudioClip;
        audio.Play();
        starMovingToFinalPoint = true;
        yield return new WaitForSeconds(spawnAudioClip.length);
        SceneManager sceneManager = (SceneManager) FindObjectOfType(typeof(SceneManager));
        worldTheme.audio.clip = sceneManager.sceneAudioClip;
        worldTheme.audio.Play ();
        tag = defaultTag;
    }

    protected IEnumerator MoveToFinalPoint() {
        starMovingToFinalPoint = true;
        yield return new WaitForSeconds(1);
        starMovingToFinalPoint = false;
    }
}
