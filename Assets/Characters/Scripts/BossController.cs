using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour {
	public float health = 3;
	public float followSpeed = 3;
	public float followAngularSpeed = 30;
	public float playerCarrySpeed = 5;							// Walking speed of player when carrying boss
	public float minHurtAltitude = 0;								// The min altitude that boss must be at to be hurt by player
	public GameObject terrain;									// Terrain that the boss stands on
	public AudioClip hurtAudioClip;								// Sound when boss gets hurt
	
	protected NavMeshAgent agent;
	protected GameObject player;
	protected CharacterMotor motor;
	protected FPSInputController playerController;
	protected PlayerHealth playerHealth;
	protected SixenseHandController[] playerHandControllers;
	protected Movement movement;
	protected float defaultHealth;
	protected float defaultSpeed;
	protected float defaultAngularSpeed;
	protected bool initBattle = false;
	protected bool startedBattle = false;
	protected bool heldByPlayer; 								// If boss has been held by player before
	protected bool isBeingThrown;								// If boss is currently being thrown
	protected bool isBeingHurt;

	// These are all the movement types that the enemy can do
	protected enum Movement{Follow, Freeze, Grab, Idle, Throw};
	
	protected virtual void Awake () {
		agent = this.GetComponent<NavMeshAgent> ();
		player = GameObject.FindWithTag("Player");
		motor = player.GetComponent<CharacterMotor> ();
		playerController = player.GetComponent<FPSInputController> ();
		playerHealth = player.GetComponent<PlayerHealth> ();
		playerHandControllers = player.GetComponentsInChildren<SixenseHandController> ();
		defaultHealth = health;
		defaultSpeed = agent.speed;
		defaultAngularSpeed = agent.angularSpeed;
		movement = Movement.Idle;
	}

	protected virtual void Start () {
		health = defaultHealth;

		heldByPlayer = false;
		isBeingThrown = false;
		isBeingHurt = false;

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
	
	protected void OnCollisionEnter(Collision col) {
		if (col.gameObject.name == terrain.name && isBeingThrown && !isBeingHurt && transform.position.y > minHurtAltitude) {
			StartCoroutine(Hurt(hurtAudioClip.length));
		}
	}
	
	protected IEnumerator Hurt (float length) {
		isBeingHurt = true;
		health -= 1;
		if (!audio.isPlaying) {
			audio.clip = hurtAudioClip;
			audio.Play();
		}
		yield return new WaitForSeconds(length);
		isBeingThrown = false;
		heldByPlayer = false;
		isBeingHurt = false;

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
			motor.movement.maxForwardSpeed = playerCarrySpeed;
			motor.movement.maxBackwardsSpeed = playerCarrySpeed;
		} else {
			motor.movement.maxForwardSpeed = playerController.getDefaultMaxForwardSpeed();
			motor.movement.maxBackwardsSpeed = playerController.getDefaultMaxForwardSpeed();
			if (!heldByPlayer) {
				agent.enabled = true;
			} else {
				if (rigidbody) {
					rigidbody.useGravity = true;
				}
				isBeingThrown = true;
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
