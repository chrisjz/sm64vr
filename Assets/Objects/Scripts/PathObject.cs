using UnityEngine;
using System.Collections;

public class PathObject : MonoBehaviour {
	public string pathName = "";
	public float pathTime = 10;
	public iTween.EaseType pathEaseType = iTween.EaseType.linear;
	public iTween.LoopType pathLoopType = iTween.LoopType.loop;
	public bool pathOrientToPath = false;

	protected float pathTimer;
	
	void Awake () {
		if (pathName == "") {
			pathName = this.GetComponent<iTweenPath> ().pathName;
		}
	}
	
	void Start () {		
		pathTimer = 0;
	}
	
	void Update () {
		pathTimer -= Time.deltaTime;		
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
		            "easetype", pathEaseType,
		            "looptype", pathLoopType,
		            "orienttopath", false));
	}
}
