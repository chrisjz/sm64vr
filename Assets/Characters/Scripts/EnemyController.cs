using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public float visionDistance = 15; 	// How far the enemy can see
	public float pathTime = 10; 		// Time taken for enemy to traverse its path

	private NavMeshAgent agent;
	private GameObject player;
	private SixenseHandController[] playerHandControllers;
	private RaycastHit hit;
	private Movement movement;
	private string pathName;
	private float pathTimer;
	private bool heldByPlayer; // If enemy has been held by player before

	// These are all the movement types that the enemy can do
	enum Movement{Path, Follow, Freeze};

	void Awake() {
		agent = this.GetComponent<NavMeshAgent> ();
		player = GameObject.FindWithTag("Player");
		playerHandControllers = player.GetComponentsInChildren<SixenseHandController> ();
		pathName = this.GetComponent<iTweenPath> ().pathName;
		pathTimer = 0;
		heldByPlayer = false;
	}

	void Start() {
		movement = Movement.Path;
	}

	void Update () {
		if (!player) {
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
	private void IsPlayerHoldingEnemy() {
		bool isHoldingEnemy = false;
		foreach (SixenseHandController playerHandController in playerHandControllers) {
			if (gameObject == playerHandController.GetClosestObject() && playerHandController.IsHoldingObject()) {
				isHoldingEnemy = true;
				heldByPlayer = true;
			}
		}

		if (isHoldingEnemy) {
			movement = Movement.Freeze;
		} else if (heldByPlayer) {
			movement = Movement.Follow;
		}
	}

	private void FollowPlayer () {
		iTween.Stop(gameObject);
		agent.SetDestination (player.transform.position);
	}

	// Check if certain object is in view of enemy. Object identified by its tag
	private void IsObjectInViewByTag( string tag) {
		if (Physics.Raycast(transform.position, this.transform.forward, out hit)) {
			if (hit.transform.tag == tag && hit.distance <= visionDistance ) {
				movement = Movement.Follow;
			}
		}
	}

	private void PathMovement() {
		if (pathTimer <= 0) {
			pathTimer = pathTime * 60;
			movement = Movement.Path;
			PathAction();
		}
	}

	private void PathAction () {
		iTween.MoveTo(gameObject,
		              iTween.Hash("path", iTweenPath.GetPath(pathName),
		            "time", pathTime,
		            "easetype", iTween.EaseType.linear,
		            "looptype", iTween.LoopType.loop,
		            "orienttopath", true));
	}

	private void Freeze () {
		agent.Stop ();
		iTween.Stop(gameObject);
	}
}
