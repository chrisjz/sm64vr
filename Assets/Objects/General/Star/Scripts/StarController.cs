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
    protected bool canGrabStar;
    protected bool grabbedStar;

    protected virtual void Awake () {
        worldTheme = GameObject.Find ("Theme");
        starMovingToFinalPoint = false;
        canGrabStar = false;
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
        if (canGrabStar &&
            (col.transform.root.gameObject.tag == "Player" ||
            col.gameObject.name == "RightHandCollider" ||
            col.gameObject.name == "LeftHandCollider" ||
            col.gameObject.GetComponentInParent<RigidHand>())) {
            if (!grabbedStar) {
                StartCoroutine(GrabStar ());
            }
            grabbedStar = true;
        }
    }

    public void Spawn () {
        GetComponent<Rigidbody>().detectCollisions = false;
        StartCoroutine(PlaySpawnAudio ());
        StartCoroutine (MoveToFinalPoint ());
    }
    
    protected IEnumerator GrabStar() {
        worldTheme.GetComponent<AudioSource>().clip = captureAudioClip;
        worldTheme.GetComponent<AudioSource>().Play ();
        yield return new WaitForSeconds(captureAudioClip.length);
        SceneManager sceneManager = (SceneManager) FindObjectOfType(typeof(SceneManager));

        if (sceneManager)
            sceneManager.SaveScore ();

        PlayerPrefs.SetString ("previousSceneName", Application.loadedLevelName);
        PlayerPrefs.SetString ("previousSceneExitAction", "exit");
        Application.LoadLevel(worldSceneName);
        
    }

    protected IEnumerator PlaySpawnAudio () {
        GetComponent<AudioSource>().clip = spawnAudioClip;
        GetComponent<AudioSource>().Play();
        starMovingToFinalPoint = true;
        yield return new WaitForSeconds(spawnAudioClip.length);
        SceneManager sceneManager = (SceneManager) FindObjectOfType(typeof(SceneManager));
        worldTheme.GetComponent<AudioSource>().clip = sceneManager.sceneAudioClip;
        worldTheme.GetComponent<AudioSource>().Play ();
        GetComponent<Rigidbody>().detectCollisions = true;
        tag = defaultTag;
        canGrabStar = true;
    }

    protected IEnumerator MoveToFinalPoint() {
        starMovingToFinalPoint = true;
        yield return new WaitForSeconds(1);
        starMovingToFinalPoint = false;
    }
}
