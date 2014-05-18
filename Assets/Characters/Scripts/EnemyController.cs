using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public float visionDistance = 15; 	// How far the enemy can see
	public float followSpeed = 12;
	public AudioClip followAudioClip;
	public float pathTime = 10; 		// Time taken for enemy to traverse its path

	protected NavMeshAgent agent;
	protected GameObject player;
	protected SixenseHandController[] playerHandControllers;
	protected RaycastHit hit;
	protected Movement movement;
	protected string pathName;
	protected float pathTimer;
	protected float defaultSpeed;
	protected bool heldByPlayer; // If enemy has been held by player before
	protected bool dead; // If enemy is dead

	// These are all the movement types that the enemy can do
	protected enum Movement{Path, Follow, Freeze};

	protected virtual void Awake() {
		agent = this.GetComponent<NavMeshAgent> ();
		player = GameObject.FindWithTag("Player");
		playerHandControllers = player.GetComponentsInChildren<SixenseHandController> ();
		pathName = this.GetComponent<iTweenPath> ().pathName;
		pathTimer = 0;
		heldByPlayer = false;
		defaultSpeed = agent.speed;
		dead = false;
	}

	void Start() {
		movement = Movement.Path;
	}

	void Update () {
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
		bool isHoldingEnemy = false;
		foreach (SixenseHandController playerHandController in playerHandControllers) {
			if (gameObject == playerHandController.GetClosestObject() && playerHandController.IsHoldingObject()) {
				isHoldingEnemy = true;
				heldByPlayer = true;
			}
		}

		if (isHoldingEnemy) {
			movement = Movement.Freeze;
			agent.enabled = false;
		} else {
			agent.enabled = true;
		}

		if (!isHoldingEnemy && heldByPlayer) {
			agent.enabled = true;
			movement = Movement.Follow;
		}
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
		if (pathTimer <= 0) {
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

	protected void Freeze () {
		iTween.Stop(gameObject);
	}

	protected IEnumerator Death (float length) {
		dead = true;
		yield return new WaitForSeconds(length);
		gameObject.SetActive (false);
	}

	protected void ToggleVisibility() {
		Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in renderers) {
			renderer.enabled = !renderer.enabled;
		}
	}
}
