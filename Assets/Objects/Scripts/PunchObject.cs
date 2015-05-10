/************************************************************************************

Filename    :   PunchObject.cs
Content     :   Object punchable by player
Created     :   16 August 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class PunchObject : MonoBehaviour {
	public string emissionName = "Particles";		// Name of object that emits particles on object punch
	public AudioClip hitAudioClip;

	protected ParticleSystem emission;
	protected bool exploding;

	void Awake () {
		Transform particleEmitter = transform.Find (emissionName);

		if (particleEmitter) {
			emission = particleEmitter.GetComponent<ParticleSystem>();
		}
	}

	void Start () {
		exploding = false;
		transform.GetComponent<Renderer>().enabled = true;
		gameObject.SetActive (true);
	}
	
    void OnCollisionEnter(Collision col) {
		if ((col.gameObject.name == "LeftHandCollider" ||
             col.gameObject.name == "RightHandCollider" ||
             col.gameObject.GetComponentInParent<RigidHand>() ||
             col.gameObject.name == "Small Block") &&
            !exploding) {
			Explode();
		}
	}

	void Explode() {
		exploding = true;

		if (!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().clip = hitAudioClip;
			GetComponent<AudioSource>().Play();
		}

		emission.Play ();
		
		transform.GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

		StartCoroutine (Destroy ());

	}

	protected IEnumerator Destroy () {
		yield return new WaitForSeconds(5);
        Destroy (gameObject);
	}
}
