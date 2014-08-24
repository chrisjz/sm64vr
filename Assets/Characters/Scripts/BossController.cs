/************************************************************************************

Filename    :   BossController.cs
Content     :   Controller for enemy boss
Created     :   9 June 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour {
    public float health = 3;
    public float followSpeed = 3;
    public float followAngularSpeed = 30;
    public float playerCarrySpeed = 5;                          // Walking speed of player when carrying boss
    public float playerCarryTurnSpeed = 2;                      // Turn speed of player when carrying boss
    public float minHurtAltitude = 0;                           // The min altitude that boss must be at to be hurt by player
    public float heldFixedRotationX;                            // Keep the boss rotated on axis X at this value when held by player
    public float hurtDuration = 3;                              // Seconds where boss is in hurt stage
    public string heldAnimationName;                            // Name of animation clip when player holds boss
    public float heldAnimationSpeed;                            // How fast animation runs when boss held by player
    public string grabAnimationName;                            // Name of animation clip when boss grabs player
    public string throwAnimationName;                           // Name of animation clip when boss throws player
    public string recoverAnimationName;	                        // Name of animation clip where boss recovers from being hurt
    public GameObject terrain;                                  // Terrain that the boss stands on
    public GameObject grabPerimeter;                            // Area where boss will grab player if player enters the area
    public float minDistanceGrabPermimeter = 4;	                // If player distance from grab perimeter less then this, player is grabbed
    public AudioClip hurtAudioClip;                             // Sound when boss gets hurt
    public float standBackUpSpeed = 0.05f;                      // Time taken for boss to stand back up when grounded
    public float lowerYBossInvincibilityBoundary;               // If boss hurt past this boundary, it receives no damage
    public float lowerYExitArenaBoundary;                       // If player leaves this boundary, boss resets
    public AudioClip explosionAudioClip;
    public Vector3 starSpawnOffset = new Vector3 (0, 2, 0);
    public float waitToSpawnStar = 0;                           // Time to wait until star is spawned after boss defeat

    protected NavMeshAgent agent;
    protected GameObject player;
    protected CharacterMotor motor;
    protected FPSInputController playerController;
    protected PlayerLook playerLook;
    protected HydraLook playerHydraLook;
    protected PlayerHealth playerHealth;
    protected HandController[] playerHandControllers;
    protected Movement movement;
    protected GameObject explosion;
    protected GameObject star;
    protected string defaultTag;                                // object's current tag, assumes this tag handles if object is grabbable
    protected string defaultAnimationName;
    protected float defaultHealth;
    protected float defaultSpeed;
    protected float defaultAngularSpeed;
    protected bool initBattle = false;
    protected bool startedBattle = false;
    protected bool isHeldByPlayer;                              // If boss is currently being held by player
    protected bool wasHeldByPlayer;                             // If boss has been held by player before
    protected bool isBeingHurt;
    protected bool isGrabbingPlayer = false;
    protected bool isThrowingPlayer = false;
    protected bool isStandingBackUp = false;
    protected bool dead;                                        // If enemy is dead

    // These are all the movement types that the enemy can do
    protected enum Movement{Follow, Freeze, Grab, Idle, Throw};

    protected GameObject startMarkerThrowPlayer;
    protected GameObject endMarkerThrowPlayer;
    protected float startTimeThrowPlayer;
    protected float journeyLengthThrowPlayer;

    protected Vector3 spawnPosition;
    protected Quaternion spawnRotation;
	
	protected virtual void Awake () {
		agent = this.GetComponent<NavMeshAgent> ();
		player = GameObject.FindWithTag("Player");
		motor = player.GetComponent<CharacterMotor> ();
		playerController = player.GetComponent<FPSInputController> ();
		playerLook = player.GetComponent<PlayerLook> ();
		playerHydraLook = player.GetComponent<HydraLook> ();
		playerHealth = player.GetComponent<PlayerHealth> ();
		playerHandControllers = player.GetComponentsInChildren<HandController> ();
		startMarkerThrowPlayer = new GameObject();
        endMarkerThrowPlayer = new GameObject ();
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        defaultAnimationName = animation.clip.name;
		defaultHealth = health;
		defaultSpeed = agent.speed;
		defaultAngularSpeed = agent.angularSpeed;
		movement = Movement.Idle;
		defaultTag = gameObject.tag;
	}

	protected virtual void Start () {
		health = defaultHealth;
        Init ();
	}

    protected void Init() {
        isHeldByPlayer = false;
        wasHeldByPlayer = false;
        isBeingHurt = false;
        
        TriggerIgnorePlayerHandColliders (true);
        
        gameObject.tag = "Untagged";

    }
	
    protected virtual void Update () {
        if (dead) {
            return;
        }

		if (initBattle && !startedBattle) {
			TriggerBattle(true);
		}
		
		IsPlayerHoldingEnemy ();

		IsPlayerInGrabPermimeter ();

        if (startedBattle) {
            IsPlayerOutsideArena();
        }

		switch (movement)
		{
		case Movement.Follow:
			FollowPlayer ();
			break;
		case Movement.Freeze:
			Freeze ();
			break;
		}

		if (isThrowingPlayer) {
			float distCovered = (Time.time - startTimeThrowPlayer) * 20.0f;
			float fracJourney = distCovered / journeyLengthThrowPlayer;
			player.transform.position = Vector3.Lerp(startMarkerThrowPlayer.transform.position, endMarkerThrowPlayer.transform.position, fracJourney);

			if (fracJourney >= 1f) {
				isGrabbingPlayer = false;
				isThrowingPlayer = false;
				animation.Play ("Walk");
			}
		}
        
        // Transition between fallen on back to standing up
        if (isStandingBackUp) {
            transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler(new Vector3 (0, transform.rotation.y, transform.rotation.z)), standBackUpSpeed);
        }
	}
	
	protected void OnCollisionEnter(Collision col) {
		if (col.gameObject.name == terrain.name && !isHeldByPlayer && wasHeldByPlayer && !isBeingHurt && transform.position.y > minHurtAltitude) {
			StartCoroutine(Hurt());
		}
	}
	
	protected IEnumerator Hurt () {
        gameObject.tag = "Untagged";
        rigidbody.constraints = RigidbodyConstraints.FreezePosition;
		isBeingHurt = true;
		health -= 1;
		if (!audio.isPlaying) {
			audio.clip = hurtAudioClip;
			audio.Play();
		}
		yield return new WaitForSeconds(hurtDuration);
        if (health != 0) {
            rigidbody.constraints &= ~RigidbodyConstraints.FreezePosition;  // unfreeze position
            rigidbody.freezeRotation = false;
            StartCoroutine (SitBackUp ());
        } else {
            StartCoroutine(Death());
        }
	}

	// Boss will return back to their feet after fallen onto ground
	protected IEnumerator SitBackUp () {
		animation.Play ("Recover");
		float animLength = animation.clip.length;

        isStandingBackUp = true;
		yield return new WaitForSeconds(animLength);
        isStandingBackUp = false;
		// Stop ignoring player colliders
		TriggerIgnorePlayerColliders(false);

		agent.enabled = true;
		wasHeldByPlayer = false;
		isBeingHurt = false;
		movement = Movement.Follow;
		animation.Play ("Walk");
        gameObject.tag = defaultTag;
    }
    
    protected virtual void TriggerBattle(bool state) {
        if (state) {
            startedBattle = true;
            rigidbody.useGravity = true;
            animation.Play ("Walk");
            StartCoroutine (StartFollowingPlayer (3));
        } else {
            startedBattle = false;
            initBattle = false;
            rigidbody.useGravity = false;
            animation.Play (defaultAnimationName);
            agent.enabled = false;
            agent.enabled = true;
            movement = Movement.Idle;
            transform.position = spawnPosition;
            transform.rotation = spawnRotation;
            Init ();
            TriggerBossBattle triggerBossBattle = (TriggerBossBattle) FindObjectOfType(typeof(TriggerBossBattle));
            triggerBossBattle.SetAudioPlaying(false);
            SceneManager sceneManager = (SceneManager) FindObjectOfType(typeof(SceneManager));
            GameObject worldTheme = GameObject.Find ("Theme");
            worldTheme.audio.clip = sceneManager.sceneAudioClip;
            worldTheme.audio.Play ();
        }
    }
	
	protected IEnumerator StartFollowingPlayer (float length) {
		agent.speed = 1;
		agent.angularSpeed = 60;
		agent.SetDestination (player.transform.position);
		yield return new WaitForSeconds(length);
		movement = Movement.Follow;
		gameObject.tag = defaultTag;

	}
	
	// If player is holding enemy then stop any enemy movement
	protected void IsPlayerHoldingEnemy() {
		if (IsHoldingEnemy ()) {
			movement = Movement.Freeze;
			LimitPlayerAbilities(true);
			rigidbody.freezeRotation = true;
			
			// Ignore player colliders when held by player
			TriggerIgnorePlayerColliders(true);
		} else {
			LimitPlayerAbilities(false);
		}
	}

	// Grab player if within grab perimeter
	protected void IsPlayerInGrabPermimeter () {
		float dist = Vector3.Distance(player.transform.position, grabPerimeter.transform.position);

		if (dist < minDistanceGrabPermimeter) {
			if (!isGrabbingPlayer) {
				StartCoroutine(GrabPlayer ());
			}
			Vector3 offset = new Vector3 (-5.0f, 3f, 0f);
			player.transform.position = transform.position + offset;
			player.transform.forward = transform.forward;
		}
	}

    protected void IsPlayerOutsideArena () {
        if (player.transform.position.y <= lowerYExitArenaBoundary) {
            TriggerBattle(false);
        }
    }

	// Boss grabs player
	protected IEnumerator GrabPlayer () {
		animation.Play (grabAnimationName);
		isGrabbingPlayer = true;
		yield return new WaitForSeconds(animation.clip.length);
		ThrowPlayer ();
	}

	protected void ThrowPlayer () {
		if (isThrowingPlayer) {
			return;
		}
		animation.Play (throwAnimationName);
		startMarkerThrowPlayer.transform.position = player.transform.position;
		endMarkerThrowPlayer.transform.position = startMarkerThrowPlayer.transform.position + new Vector3 (-20f, 10f, 0f);
		startTimeThrowPlayer = Time.time;
		journeyLengthThrowPlayer = Vector3.Distance(startMarkerThrowPlayer.transform.position, endMarkerThrowPlayer.transform.position);
		isThrowingPlayer = true;
	}
	
	protected bool IsHoldingEnemy () {
		foreach (HandController playerHandController in playerHandControllers) {
			if (gameObject == playerHandController.GetClosestObject() && playerHandController.IsHoldingObject()) {
				isHeldByPlayer = true;
				wasHeldByPlayer = true;
				animation.Play (heldAnimationName);
				animation[heldAnimationName].speed = heldAnimationSpeed;
				if (heldFixedRotationX >= -360 && heldFixedRotationX <= 360) {
					transform.rotation = Quaternion.Euler(new Vector3 (heldFixedRotationX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
				}
				return true;
			}
		}
		
		isHeldByPlayer = false;
		return false;
	}

    // Trigger slowing down and preventing jumping for player
	protected void LimitPlayerAbilities (bool change) {
		if (change) {
			motor.movement.maxForwardSpeed = playerCarrySpeed;
			motor.movement.maxBackwardsSpeed = playerCarrySpeed;
			playerLook.sensitivityX = playerCarryTurnSpeed;
			playerHydraLook.sensitivityX = playerCarryTurnSpeed;
            playerController.JumpEnabled = false;
		} else {
			motor.movement.maxForwardSpeed = playerController.getDefaultMaxForwardSpeed();
			motor.movement.maxBackwardsSpeed = playerController.getDefaultMaxForwardSpeed();
			playerLook.sensitivityX = playerLook.getDefaultSensitivityX();
            playerHydraLook.sensitivityX = playerHydraLook.getDefaultSensitivityX();
            playerController.JumpEnabled = true;
		}
	}

	protected virtual void FollowPlayer() {
		agent.SetDestination (player.transform.position);
		agent.speed = followSpeed;
		agent.angularSpeed = followAngularSpeed;
	}
	
	protected virtual void Freeze () {
		agent.enabled = false;
	}

	protected void TriggerIgnorePlayerHandColliders (bool state) {
		foreach (HandController playerHandController in playerHandControllers) {
			Collider[] cols = GetComponentsInChildren<Collider>();
			Collider[] playerHandCols = playerHandController.GetComponentsInChildren<Collider>();
			
			foreach (Collider playerHandCol in playerHandCols) {
				foreach (Collider col in cols) {
					Physics.IgnoreCollision(playerHandCol, col, state);
				}
			}
		}
	}

	protected void TriggerIgnorePlayerColliders (bool state) {
		Collider[] cols = GetComponentsInChildren<Collider>();
		Collider[] playerCols = player.GetComponentsInChildren<Collider>();
		
		foreach (Collider playerCol in playerCols) {
			foreach (Collider col in cols) {
				Physics.IgnoreCollision(playerCol, col, state);
			}
		}
		
		// Always ignore player hand colliders
		if (!state) {
			TriggerIgnorePlayerHandColliders(true);
		}
	}
	
	public void SetInitBattle(bool state) {
		initBattle = state;
	}

    protected IEnumerator Death () {
        dead = true;
        yield return new WaitForSeconds(waitToSpawnStar);
        explosion = (GameObject) Instantiate(Resources.Load("Explosion"));
        explosion.transform.position = transform.position;
        TriggerBossBattle triggerBossBattle = (TriggerBossBattle) FindObjectOfType(typeof(TriggerBossBattle));
        triggerBossBattle.worldTheme.audio.Stop ();
        Disable ();
        SpawnStar ();
    }

    protected void SpawnStar () {
        star = (GameObject) Instantiate(Resources.Load("Star"));
        StarController starController = star.GetComponent<StarController> ();
        star.transform.position = transform.position + starSpawnOffset;
        starController.Spawn ();

    }
    
    protected void Disable() {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
            renderer.enabled = false;
        }
        
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders) {
            col.enabled = false;
        }
    }
}
