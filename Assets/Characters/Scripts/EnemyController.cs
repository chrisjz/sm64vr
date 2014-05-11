using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public float visionDistance = 15;
	private NavMeshAgent agent;
	private GameObject player;
	private RaycastHit hit;
	private bool followPlayer = false;

	void Awake() {
		agent = this.GetComponent<NavMeshAgent> ();
		player = GameObject.FindWithTag("Player");
	}

	void Update () {
		if (player) {
			if (Physics.Raycast(transform.position, this.transform.forward, out hit)) {
				if (hit.transform.tag == "Player" && hit.distance <= visionDistance ) {
					followPlayer = true;
				}
			}

			if (followPlayer) {
				FollowPlayer();
			}
		}
	}

	private void FollowPlayer () {
		agent.SetDestination (player.transform.position);
	}
}
