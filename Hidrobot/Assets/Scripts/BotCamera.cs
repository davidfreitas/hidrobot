using UnityEngine;
using System.Collections;

public class BotCamera : MonoBehaviour {

	public Transform target;

	// Use this for initialization
	void Start () {
		Invoke ("StopAnimation", 2);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = target.position;
		pos.x -= 35;
		pos.y += 55;
		pos.z -= 35;
		transform.position = pos;
		transform.LookAt (target);
	}

	void StopAnimation(){
		Animator a = GetComponent<Animator> ();
		a.enabled = false;
	}
}
