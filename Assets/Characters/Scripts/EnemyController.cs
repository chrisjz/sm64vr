/************************************************************************************

Filename    :   EnemyController.cs
Content     :   Controller for enemy
Created     :   11 May 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public float visionDistance = 15; 				// How far the enemy can see
	public float followSpeed = 12;
	public AudioClip followAudioClip;
	public float pathTime = 10; 					// Time taken for enemy to traverse its path
	public int playerDamage = 1;					// Amount of damage player receives when hit by enemy
	public float respawnTime = 1;					// Time until enemy respawns after death. Will not respawn if set to 0.
	public float knockbackOtherForce = 30;			// Distance of how much a victim is knocked back on collission with enemy
	public float knockbackEnemyForce = 50;			// Distance of how much enemy is knocked back on collission with other collider
	public float minKnockbackEnemyForce = 400;		// The minimum distance the enemy is knocked back on collission with other collider
	public float reboundForce = 10;					// Rebound force on player
	public float knockbackDuration = 1;				// Duration of enemy being knocked back

	protected NavMeshAgent agent;
	protected GameObject player;
    protected PlayerHealth playerHealth;
    protected SixenseHandControllerExtender[] playerSixsenseHandControllers;
	protected RaycastHit hit;
	protected Movement movement;
	protected string initAnimationName;
	protected string pathName;
	protected float pathTimer;
	protected float speed;
	protected float defaultSpeed;
	protected bool heldByPlayer; 								// If enemy has been held by player before
	protected bool knockingBack;								// If enemy is currently being knocked back
	protected bool dead; 										// If enemy is dead

	// These are all the movement types that the enemy can do
	protected enum Movement{Path, Follow, Freeze};

	protected Vector3 spawnPosition;
	protected Quaternion spawnRotation;

	protected virtual void Awake() {
		agent = this.GetComponent<NavMeshAgent> ();
		player = GameObject.FindWithTag("Player");
		playerHealth = player.GetComponent<PlayerHealth> ();
		playerSixsenseHandControllers = player.GetComponentsInChildren<SixenseHandControllerExtender> ();

        if (this.GetComponent<iTweenPath> ())
		    pathName = this.GetComponent<iTweenPath> ().pathName;

		defaultSpeed = agent.speed;
		initAnimationName = animation.clip.name;
		spawnPosition = transform.position;
		spawnRotation = transform.rotation;
	}

	protected virtual void Start() {
		Init ();
	}

	protected virtual void Init () {
		movement = Movement.Path;
		pathTimer = 0;
		heldByPlayer = false;
		knockingBack = false;
		dead = false;
	}

	protected virtual void Update () {
		if (!player || dead) {
			return;
		}

		pathTimer -= Time.deltaTime;

		IsPlayerHoldingEnemy ();

		switch (movement)
		{
			case Movement.Follow:
				FollowPlayer ();
				break;
			case Movement.Freeze:
				Freeze ();
				break;
			case Movement.Path:
				PathMovement();
				break;
		}

		if (movement == Movement.Path) {
			IsObjectInViewByTag("Player");
		}
	}
	
	// If player is holding enemy then stop any enemy movement
	protected void IsPlayerHoldingEnemy() {
		if (!heldByPlayer && IsHoldingEnemy ()) {
			movement = Movement.Freeze;
			agent.enabled = false;
			heldByPlayer = true;
		} else if (!heldByPlayer) {
			agent.enabled = true;
		}
	}

	protected bool IsHoldingEnemy () {
        foreach (SixenseHandControllerExtender playerHandController in playerSixsenseHandControllers) {
			if (gameObject == playerHandController.GetClosestObject() && playerHandController.IsHoldingObject()) {
				return true;
			}
		}

        LeapHandGrabExtender[] leapHands = GameObject.FindObjectsOfType<LeapHandGrabExtender> ();

        foreach (LeapHandGrabExtender leapHand in leapHands) {
            if (leapHand.Grabbed_ && leapHand.Grabbed_.gameObject == gameObject) {
                return true;
            }
        }

		return false;
	}

	protected virtual void FollowPlayer () {
		iTween.Stop(gameObject);

		agent.SetDestination (player.transform.position);
		agent.speed = followSpeed;

		if (!audio.isPlaying) {
			audio.clip = followAudioClip;
			audio.Play();
		}
	}

	// Check if certain object is in view of enemy. Object identified by its tag
	protected void IsObjectInViewByTag( string tag) {
		if (Physics.Raycast(transform.position, this.transform.forward, out hit)) {
			if (hit.transform.tag == tag && hit.distance <= visionDistance ) {
				movement = Movement.Follow;
			}
		}
	}

	protected void PathMovement() {
        if (pathTimer <= 0 && this.GetComponent<iTweenPath> ()) {
			pathTimer = pathTime * 60;
			movement = Movement.Path;
			PathAction();
		}
	}

	protected void PathAction () {
		iTween.MoveTo(gameObject,
		              iTween.Hash("path", iTweenPath.GetPath(pathName),
		            "time", pathTime,
		            "easetype", iTween.EaseType.linear,
		            "looptype", iTween.LoopType.loop,
		            "orienttopath", true));
	}

	protected virtual void Freeze () {
		iTween.Stop(gameObject);
	}
	
	public virtual void Knockback(GameObject knocker, GameObject victim, GameObject collider = null, float forceMultiplier = 1) {
		Vector3 dir = (victim.transform.position - knocker.transform.position).normalized;

		if (victim == player) {
			CharacterMotor charMotor = player.GetComponent<CharacterMotor> ();
			dir.y = 0;

			charMotor.SetVelocity (dir * knockbackOtherForce * forceMultiplier);
			DamagePlayer();
		} else if (victim.rigidbody && !knockingBack) {
			float force = knockbackEnemyForce;

			if (collider) {
				// Increase the force if the collider was part of a hand object
				force *= GetKnockersHandVelocity(collider);
			}

			force *= forceMultiplier;

			if (minKnockbackEnemyForce > force) {
				force = minKnockbackEnemyForce;
			}

			movement = Movement.Freeze;
			victim.rigidbody.AddForce(dir * force);
			knockingBack = true;
			StartCoroutine(KnockbackEnemy(knockbackDuration));
		}
	}
	
	protected IEnumerator KnockbackEnemy (float length) {
		animation.Stop ();
		yield return new WaitForSeconds(length);
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.Sleep();
		knockingBack = false;
		dead = true;
		ToggleVisibility ();
		animation.Play ();
		StartCoroutine(Death(1));
	}

	protected float GetKnockersHandVelocity (GameObject collider) {
        SixenseHandControllerExtender hand = collider.transform.parent.gameObject.GetComponent<SixenseHandControllerExtender>();
		
		if (hand) {
			return hand.GetHandVelocity();
		}

		return 1;
	}

	protected void DamagePlayer() {

		if (playerHealth) {
			playerHealth.Damage(playerDamage);
		}
	}
	
	// Friendly bounce back of player on specific actions with enemy
	protected void ReboundPlayer(bool pushUp = false) {
		Vector3 dir = (player.transform.position - transform.position).normalized;

		if (pushUp) {
			dir.y = 1.0f;
		}

		CharacterMotor charMotor = player.GetComponent<CharacterMotor> ();
		
		charMotor.SetVelocity (dir * reboundForce);
	}
	
	protected IEnumerator Death (float length) {
		dead = true;
		yield return new WaitForSeconds(length);
		if (respawnTime >= 0)
            StartCoroutine (Respawn (respawnTime));
        else
            Destroy (gameObject);
	}

    protected IEnumerator Respawn (float length) {
		yield return new WaitForSeconds(length);
		gameObject.transform.position = spawnPosition;
        gameObject.transform.rotation = spawnRotation;
		agent.speed = defaultSpeed;
		animation.Play (initAnimationName);
		Init ();
		ToggleVisibility ();
	}
	
	protected void ToggleVisibility() {
		Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in renderers) {
			renderer.enabled = !renderer.enabled;
		}

		Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
		foreach (Collider col in colliders) {
			col.enabled = !col.enabled;
		}
	}
}
