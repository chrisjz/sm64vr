/************************************************************************************

Filename    :   CoinController.cs
Content     :   Controller for coin object
Created     :   28 September 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour {
    public enum Type {Blue, Red, Yellow}
    public Type type = Type.Yellow;
    public AudioClip yellowCoinSound;

    protected SceneManager sceneManager;
    protected GameObject player;

    void Awake () {
        sceneManager = GameObject.FindObjectOfType<SceneManager> ();
    }

    protected void OnTriggerEnter(Collider col) {
        if (col.transform.root.gameObject.tag == "Player" || col.gameObject.GetComponentInParent<RigidHand>()) {
            player = col.transform.root.gameObject;
            StartCoroutine(Collect ());
        }

    }

    protected IEnumerator Collect () {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth> ();
        int value = GetCoinValue ();
        gameObject.collider.enabled = false;
        gameObject.renderer.enabled = false;
        audio.clip = yellowCoinSound;
        audio.Play ();
        yield return new WaitForSeconds(audio.clip.length);
        playerHealth.Heal (value);
        if (sceneManager) {
            sceneManager.coins += value;
            if (type == Type.Red)
                sceneManager.redCoins += 1;
        }
        gameObject.SetActive (false);
    }

    protected int GetCoinValue () {
        switch (type) {
        case Type.Blue:
            return 5;
            break;
        case Type.Red:
            return 2;
            break;
        case Type.Yellow:
        default:
            return 1;
            break;
        }
    }
}
