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

    public void Spawn () {
        StartCoroutine(PlaySpawnAudio ());
    }

    protected IEnumerator PlaySpawnAudio () {        
        audio.clip = spawnAudioClip;
        audio.Play();
        yield return new WaitForSeconds(spawnAudioClip.length);
        GameObject worldTheme = GameObject.Find ("Theme");
        SceneManager sceneManager = (SceneManager) FindObjectOfType(typeof(SceneManager));
        worldTheme.audio.clip = sceneManager.sceneAudioClip;
        worldTheme.audio.Play ();
    }
}
