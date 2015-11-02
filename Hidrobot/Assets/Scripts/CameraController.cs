using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public static CameraController controller;

	public Transform target;
	public float animationPos;

	void Awake () {
		if (controller == null) {
			controller = this;
		} else if (controller != this) {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 targetPos = target.position;
		Vector3 cameraPos;
		if (MeterBoxController.controller.state == MeterBoxController.State.Closed) {
			cameraPos = new Vector3 (targetPos.x + 4 - (Mathf.Clamp(animationPos, 0.0f, 1.0f) * 4f), targetPos.y + 4.5f + (Mathf.Clamp(animationPos, 0.0f, 1.0f) * -2.0f), targetPos.z - 5 + (Mathf.Clamp(animationPos, 0.0f, 1.0f) * 4.6f));
		} else if (MeterBoxController.controller.state == MeterBoxController.State.Open) {
			cameraPos = new Vector3 (targetPos.x + 4 - ((1.0f + Mathf.Clamp(animationPos, -1.0f, 0.0f)) * 4f), targetPos.y + 4.5f + ((1.0f + Mathf.Clamp(animationPos, -1.0f, 0.0f)) * -2.0f), targetPos.z - 5 + ((1.0f + Mathf.Clamp(animationPos, -1.0f, 0.0f)) * 4.6f));
		} else {
			cameraPos = new Vector3 (targetPos.x, targetPos.y + 2.5f, targetPos.z - 0.4f);
		}

		transform.position = cameraPos;
		transform.LookAt (target);
	}
}
