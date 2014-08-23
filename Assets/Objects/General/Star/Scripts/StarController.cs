/************************************************************************************

Filename    :   StarController.cs
Content     :   Controller for star object
Created     :   23 August 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class StarController : MonoBehaviour {
    public AudioClip spawnAudioClip;
    public Vector3 finalPointOffset = new Vector3 (0, 1.5f, 0);
    public float speed = 3f;

    protected bool starMovingToFinalPoint;

    protected virtual void Awake () {
        starMovingToFinalPoint = false;
    }

    protected virtual void Update () {
        if (starMovingToFinalPoint) {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, transform.position + finalPointOffset, step);
        }
    }

    public void Spawn () {
        StartCoroutine(PlaySpawnAudio ());
        StartCoroutine (MoveToFinalPoint ());
    }

    protected IEnumerator PlaySpawnAudio () {        
        audio.clip = spawnAudioClip;
        audio.Play();
        starMovingToFinalPoint = true;
        yield return new WaitForSeconds(spawnAudioClip.length);
        GameObject worldTheme = GameObject.Find ("Theme");
        SceneManager sceneManager = (SceneManager) FindObjectOfType(typeof(SceneManager));
        worldTheme.audio.clip = sceneManager.sceneAudioClip;
        worldTheme.audio.Play ();
    }

    protected IEnumerator MoveToFinalPoint() {
        starMovingToFinalPoint = true;
        yield return new WaitForSeconds(1);
        starMovingToFinalPoint = false;
    }
}
