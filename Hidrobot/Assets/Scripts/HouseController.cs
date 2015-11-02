using UnityEngine;
using System.Collections;

public class HouseController : MonoBehaviour {
	private bool triggered = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (GameControl.controller.state == GameControl.State.Bot && triggered && Input.anyKey) {
			GameControl.controller.ObjectiveReached();
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if(col.gameObject.tag == "Player" && GameControl.controller.objective == transform)
		{
			triggered = true;

		}
	}

	void OnTriggerExit (Collider col)
	{
		triggered = false;
	}
}
