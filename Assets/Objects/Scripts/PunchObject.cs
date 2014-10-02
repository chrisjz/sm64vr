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
		transform.renderer.enabled = true;
		gameObject.SetActive (true);
	}
	
    void OnCollisionEnter(Collision col) {
		if ((col.gameObject.name == "LeftHandCollider" ||
             col.gameObject.name == "RightHandCollider" ||
             col.gameObject.GetComponentInParent<RigidHand>()) &&
            !exploding) {
			Explode();
		}
	}

	void Explode() {
		exploding = true;

		if (!audio.isPlaying) {
			audio.clip = hitAudioClip;
			audio.Play();
		}

		emission.Play ();
		
		transform.renderer.enabled = false;
        collider.enabled = false;

		StartCoroutine (Destroy ());

	}

	protected IEnumerator Destroy () {
		yield return new WaitForSeconds(5);
        Destroy (gameObject);
	}
}
