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
            Collect ();
        }

    }

    protected void Collect () {
        switch (type) {
        case Type.Yellow:
        default:
            StartCoroutine(CollectYellowCoin ());
            break;

        }
    }

    protected IEnumerator CollectYellowCoin () {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth> ();
        gameObject.collider.enabled = false;
        gameObject.renderer.enabled = false;
        audio.clip = yellowCoinSound;
        audio.Play ();
        yield return new WaitForSeconds(audio.clip.length);
        playerHealth.Heal (1);
        if (sceneManager) {
            sceneManager.coins += 1;
        }
        gameObject.SetActive (false);
    }
}
