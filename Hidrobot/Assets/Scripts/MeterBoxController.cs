using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MeterBoxController : MonoBehaviour {

	public static MeterBoxController controller;

	public Transform lid;

	private float lh, lv, rh, rv;
	private float last_lh, last_lv, last_rh, last_rv;

	public enum State {
		Closed,
		Open,
		Pointers
	}
	
	public State state;

	private bool playedOpenSound = false;

	private bool isRotationH;
	private bool isRotating;

	public Transform lPointer;
	public Transform rPointer;
	public float pRotationSpeed = 1.0f;
	private float pRotationDirection = 1.0f;

	public AudioClip[] lidSounds = new AudioClip[2];

	private float[] lAngles = new float[3];
	private float[] rAngles = new float[3];
	private int lValue, rValue;

	private bool isOn = false;

	public Image lHighlight, rHighlight;
	private bool lOk, rOk = false;
	public Text countdownText;
	private float countdown;

	void Awake () {
		if (controller == null) {
			controller = this;
		} else if (controller != this) {
			Destroy(gameObject);
		}
	}

	void Start () {
		state = State.Closed;
		lAngles [0] = 60.0f;
		lAngles [1] = 30.0f;
		lAngles [2] = 0.0f;

		rAngles [0] = 60.0f;
		rAngles [1] = 30.0f;
		rAngles [2] = 0.0f;
	}

	void SetValues () {
		countdownText.text = "!";
		lValue = (int)Mathf.Round (Random.Range (0, 2));
		rValue = (int)Mathf.Round (Random.Range (0, 2));

		Vector3 rot = lHighlight.rectTransform.rotation.eulerAngles;
		rot.z = lAngles [lValue];
		lHighlight.rectTransform.rotation = Quaternion.Euler(rot);

		rot = rHighlight.rectTransform.rotation.eulerAngles;
		rot.z = rAngles [rValue];
		rHighlight.rectTransform.rotation = Quaternion.Euler(rot);

		isOn = true;
		countdown = 3.0f;
	}

	void ResetHighlights () {
		Vector3 rot = lHighlight.rectTransform.rotation.eulerAngles;
		rot.z = -lAngles [lValue];
		lHighlight.rectTransform.rotation = Quaternion.Euler(rot);
		
		rot = rHighlight.rectTransform.rotation.eulerAngles;
		rot.z = -rAngles [rValue];
		rHighlight.rectTransform.rotation = Quaternion.Euler(rot);
		countdown = 3.0f;
	}
	
	void FixedUpdate () {
		if (GameControl.controller.state != GameControl.State.MeterBox) {
			return;
		} else if (state == State.Pointers && !isOn) {
			SetValues();
		}

		UpdateInput ();



		if (state == State.Closed) {

		} else if (state == State.Open) {

		}
	}

	void LateUpdate () {
		if (GameControl.controller.state != GameControl.State.MeterBox) {
			return;
		}

		last_lh = lh;
		last_lv = lv;
		last_rh = rh;
		last_rv = rv;
	}

	void Update () {
		if (GameControl.controller.state != GameControl.State.MeterBox) {
			return;
		}

		if (state == State.Open || state == State.Closed) {
			UpdateLidOpening ();
		}

		if (state == State.Pointers) {
			UpdatePointers ();
		}
	}

	void UpdateInput () {
		lh = Input.GetAxis ("LeftHorizontal");
		lv = Input.GetAxis ("LeftVertical");
		rh = Input.GetAxis ("RightHorizontal");
		rv = Input.GetAxis ("RightVertical");
	}

	void UpdateLidOpening () {
		Quaternion lidRotation;
		if (state == State.Closed && lv >= 0.0f) {
			lidRotation = Quaternion.Euler (new Vector3 (100.0f * lv, 0, 0));
		} else if (state == State.Open && lv <= 0.0f) {
			lidRotation = Quaternion.Euler (new Vector3 (100.0f * (1.0f + lv), 0, 0));
		} else {
			lidRotation = lid.rotation;
		}
		lid.rotation = lidRotation;


		if (state == State.Closed && last_lv >= 0.2f && last_lv < lv && !playedOpenSound) {
			AudioSource audio = GetComponent<AudioSource> ();
			audio.clip = lidSounds[0];
			audio.Play ();
			playedOpenSound = true;
		} else if (state == State.Closed && lv == 0) {
			playedOpenSound = false;
		}

		CameraController.controller.animationPos = lv;

		if (lv >= .9f) {
			if (state == State.Closed && !isOn) {
				state = State.Pointers;
			}
		} if (state == State.Open && lv <= -.9f) {
			state = State.Closed;
			GameControl.controller.MeterBoxFinished();
			isOn = false;
			ResetHighlights();
		}


	}

	void UpdatePointers () {

		float lFactor, rFactor;
		// left pointer
		Vector3 rot = lPointer.rotation.eulerAngles;
		float angle = rot.y;
		float factor = 0.5f;
		if (lh + lv >= 1.0f) {
			if (lh > lv) {
				factor = (lh - lv) * 0.5f + 0.5f;
			} else {
				factor = (lh + lv) * 0.5f - 0.5f;
			}

			angle = Mathf.Lerp (168.0f, 268.0f, factor);
		}
		lFactor = factor;
		rot.y = Mathf.Clamp (angle + (pRotationSpeed * pRotationDirection * Time.deltaTime), 168.0f, 268.0f);
		lPointer.rotation = Quaternion.Euler (rot);

		// right pointer
		rot = rPointer.rotation.eulerAngles;
		angle = rot.y;
		factor = 0.5f;
		if (rh <= 0.0f && rv >= 0.0f && Mathf.Abs (rh) + rv >= 1.0f) {
			if (Mathf.Abs (rh) > Mathf.Abs (rv)) {
				factor = (1.0f + rh + rv) * 0.5f;
			} else {
				factor = (rh + rv) * 0.5f + 0.5f;
			}

			angle = Mathf.Lerp (80.0f, 173.0f, factor);
		}
		rFactor = factor;

		rot.y = Mathf.Clamp (angle + (pRotationSpeed * pRotationDirection * Time.deltaTime), 80.0f, 173.0f);
		rPointer.rotation = Quaternion.Euler (rot);

		if (Random.value > 0.5f) {
			pRotationDirection = 1.0f;
		} else {
			pRotationDirection = -1.0f;
		}

		pRotationSpeed = Random.Range (90.0f, 180.0f);


		if ((rValue == 0 && rFactor <= 0.33f) || (rValue == 1 && rFactor >= 0.33f && rFactor <= 0.66f) || (rValue == 2 && rFactor >= 0.66)) {
			rOk = true;
		} else {
			rOk = false;
		}
		Debug.Log (rValue);
		if ((lValue == 0 && lFactor <= 0.33f) || (lValue == 1 && lFactor >= 0.33f && lFactor <= 0.66f) || (lValue == 2 && lFactor >= 0.66)) {
			lOk = true;
		} else {
			lOk = false;
		}

		if (lOk && rOk && (lh > 0.0f || lv > 0.0f) && (rh > 0.0f || rv > 0.0f)) {
			countdown -= Time.deltaTime;
			countdownText.text = string.Format ("{0}", Mathf.Ceil (countdown));
		} else {
			countdown = 3.0f;
			countdownText.text = "!";
		}

		if (countdown <= 0.0f) {
			state = State.Open;
			countdownText.text = "OK!";
			GameControl.controller.StopAudio();
			GameControl.controller.PlaySfx(1);
		}
	}

	void UpdateTubeRotation () {
		if (rh == 1.0f) {

		}
	}
}
