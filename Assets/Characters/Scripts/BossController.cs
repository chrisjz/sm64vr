using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour {
	public float health = 3;
	public float followSpeed = 3;
	public float followAngularSpeed = 30;
	
	protected NavMeshAgent agent;
	protected GameObject player;
	protected PlayerHealth playerHealth;
	protected SixenseHandController[] playerHandControllers;
	protected Movement movement;
	protected float defaultSpeed;
	protected float defaultAngularSpeed;
	protected bool initBattle = false;
	protected bool startedBattle = false;
	protected bool heldByPlayer; 								// If boss has been held by player before

	// These are all the movement types that the enemy can do
	protected enum Movement{Follow, Freeze, Grab, Idle, Throw};
	
	protected virtual void Awake () {
		agent = this.GetComponent<NavMeshAgent> ();
		player = GameObject.FindWithTag("Player");
		playerHealth = player.GetComponent<PlayerHealth> ();
		playerHandControllers = player.GetComponentsInChildren<SixenseHandController> ();
		defaultSpeed = agent.speed;
		defaultAngularSpeed = agent.angularSpeed;
		movement = Movement.Idle;
	}

	protected virtual void Start () {
		heldByPlayer = false;

		IgnorePlayerHandColliders ();
	}
	
	protected virtual void Update () {
		if (initBattle && !startedBattle) {
			StartBattle();
		}
		
		IsPlayerHoldingEnemy ();

		switch (movement)
		{
		case Movement.Follow:
			FollowPlayer ();
			break;
		case Movement.Freeze:
			Freeze ();
			break;
		}
	}
	
	protected virtual void StartBattle() {
		startedBattle = true;
		rigidbody.useGravity = true;
		animation.Play ("Walk");
		StartCoroutine(StartFollowingPlayer(3));
	}
	
	protected IEnumerator StartFollowingPlayer (float length) {
		agent.speed = 1;
		agent.angularSpeed = 60;
		agent.SetDestination (player.transform.position);
		yield return new WaitForSeconds(length);
		movement = Movement.Follow;

	}
	
	// If player is holding enemy then stop any enemy movement
	protected void IsPlayerHoldingEnemy() {
		if (IsHoldingEnemy ()) {
			movement = Movement.Freeze;
		} else {
			if (!heldByPlayer) {
				agent.enabled = true;
			} else {
				if (rigidbody) {
					rigidbody.useGravity = true;
				}
			}
		}
	}
	
	protected bool IsHoldingEnemy () {
		foreach (SixenseHandController playerHandController in playerHandControllers) {
			if (gameObject == playerHandController.GetClosestObject() && playerHandController.IsHoldingObject()) {
				heldByPlayer = true;
				return true;
			}
		}
		
		return false;
	}

	protected virtual void FollowPlayer() {
		agent.SetDestination (player.transform.position);
		agent.speed = followSpeed;
		agent.angularSpeed = followAngularSpeed;
	}
	
	protected virtual void Freeze () {
		agent.enabled = false;
	}

	protected void IgnorePlayerHandColliders () {
		foreach (SixenseHandController playerHandController in playerHandControllers) {
			Collider[] cols = playerHandController.GetComponentsInChildren<Collider>();
			
			foreach (Collider col in cols) {
				Physics.IgnoreCollision(col, collider);
			}
		}
	}
	
	public void SetInitBattle(bool state) {
		initBattle = state;
	}
}
