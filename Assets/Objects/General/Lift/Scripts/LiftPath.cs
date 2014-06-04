using UnityEngine;
using System.Collections;

public class LiftPath : MonoBehaviour {
	public float pathTime = 8;

	protected string pathName;
	protected float pathTimer;

	void Awake () {
		pathName = this.GetComponent<iTweenPath> ().pathName;
	}

	void Start () {		
		pathTimer = 0;
	}

	void Update () {
		pathTimer -= Time.deltaTime;

		transform.eulerAngles=new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);

		PathMovement();
	}
	
	protected void PathMovement() {
		if (pathTimer <= 0) {
			pathTimer = pathTime * 60;
			PathAction();
		}
	}
	
	protected void PathAction () {
		iTween.MoveTo(gameObject,
		              iTween.Hash("path", iTweenPath.GetPath(pathName),
		            "time", pathTime,
		            "easetype", iTween.EaseType.linear,
		            "looptype", iTween.LoopType.loop,
		            "orienttopath", false));
	}
}
