using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public float visionDistance = 15;
	public float pathTime = 10;
	private NavMeshAgent agent;
	private GameObject player;
	private RaycastHit hit;
	private bool followPlayer = false;
	private string pathName;
	private float pathTimer;

	void Awake() {
		agent = this.GetComponent<NavMeshAgent> ();
		player = GameObject.FindWithTag("Player");
		pathName = this.GetComponent<iTweenPath> ().pathName;
		pathTimer = 0;
	}

	void Start() {

	}

	void Update () {
		pathTimer -= Time.deltaTime;
		if (player) {
			if (!followPlayer) {
				if (Physics.Raycast(transform.position, this.transform.forward, out hit)) {
					if (hit.transform.tag == "Player" && hit.distance <= visionDistance ) {
						followPlayer = true;
						iTween.Stop(gameObject);
					}
				}
			} else {				
				FollowPlayer ();
			}
		}

		if (pathTimer <= 0 && !followPlayer) {
			pathTimer = pathTime * 60;
			PathAction();
		}
	}

	private void FollowPlayer () {
		agent.SetDestination (player.transform.position);
	}

	private void PathAction () {
		iTween.MoveTo(gameObject,
		              iTween.Hash("path", iTweenPath.GetPath(pathName),
		            "time", pathTime,
		            "easetype", iTween.EaseType.linear,
		            "looptype", iTween.LoopType.loop,
		            "orienttopath", true));
	}
}
