using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {
	public static GameControl controller;

	public Transform arrow;
	public Transform[] houses = new Transform[9];
	public Transform objective;

	public GameObject[] cameras = new GameObject[2];

	public GameObject[] hoses = new GameObject[6];
	private float hoseCountdown = 5.0f;
	private GameObject hose;
	private bool isHosing = false;
	public Transform player;

	public AudioClip[] sounds = new AudioClip[6];

	public enum State {
		Bot,
		MeterBox
	}
	
	public State state;

	public Text text, scoreText, countdownText, gameOverText;
	public Image image;
	public int tutostage = 0;
	public int tuto = 0;
	public bool paused = false;
	public bool firstHosing = true;
	public bool firstMeter = true;
	public Sprite[] sprites = new Sprite[3];

	private float countdown = 30.0f;
	private int score = 0;
	void Awake () {
		if (controller == null) {
			controller = this;
		} else if (controller != this) {
			Destroy(gameObject);
		}
	}

	void Start () {
		objective = houses[(int)Random.Range(0,2)];
		state = State.Bot;
		hose = hoses [0];
		firstTuto (tuto);
		PlayAudio (3);
	}
	

	void Update () {
		countdownText.text = string.Format ("{0:00.0}", countdown);
		scoreText.text = string.Format ("{0}", score);
		if (paused) {
			if (gameOverText.enabled && Input.anyKey) {
				Application.LoadLevel("Title");
			}

			if (Input.anyKeyDown) {
				tutostage += 1;
			}
			firstTuto(tuto);
			return;
		}

		if (countdown <= 0.0f) {
			paused = true;
			gameOverText.enabled = true;
			countdownText.enabled = false;

		}

		countdown -= Time.deltaTime;

		arrow.LookAt (objective.position);

		if (!isHosing && state == State.Bot) {
			hoseCountdown -= Time.deltaTime;
		}

		if (hoseCountdown <= 0.0f) {
			hoseCountdown = Random.Range(5, 10);
			hose.SetActive(true);
			isHosing = true;
			PlaySfx(2);

			if (firstHosing) {
				tuto = 1;
				tutostage = 0;
				firstHosing = false;
				firstTuto(1);
			}
		}

		if (isHosing) {
			AudioSource audio = GetComponents<AudioSource> ()[1];
			if (!audio.isPlaying) {
				PlaySfx(2);
			}
			Vector3 distance = player.position - hose.transform.position;
			if(distance.magnitude < 10.0f && Input.anyKey) {
				hose.SetActive(false);
				hose = hoses[(int)Mathf.Round(Random.Range(0,5))];
				isHosing = false;
				PlaySfx(4);
				score += 500;
				countdown += 5.0f;
			}
		}
	}

	public void PlayAudio(int index) {
		AudioSource audio = GetComponents<AudioSource> ()[0];
		audio.clip = sounds[index];
		audio.Play ();
	}

	public void StopAudio() {
		AudioSource audio = GetComponents<AudioSource> ()[0];
		audio.Stop ();
	}

	public void PlaySfx(int index) {
		AudioSource audio = GetComponents<AudioSource> ()[1];
		audio.clip = sounds[index];
		audio.Play ();
	}

	public void StopSfx() {
		AudioSource audio = GetComponents<AudioSource> ()[1];
		audio.Stop ();
	}

	public void ObjectiveReached () {
		objective = houses[(int)Random.Range(0,8)];
		GameObject camObj = cameras [1];
		camObj.SetActive (true);
		Camera cam = camObj.GetComponent<Camera> ();
		cam.enabled = true;

		camObj = cameras [0];
		cam = camObj.GetComponent<Camera> ();
		cam.enabled = false;
		camObj.SetActive (false);
		state = State.MeterBox;

		PlayAudio (0);

		if (firstMeter) {
			tuto = 2;
			tutostage = 0;
			paused = false;
			firstTuto(tuto);
		}
	}

	public void MeterBoxFinished () {
		GameObject camObj = cameras [0];
		Camera cam = camObj.GetComponent<Camera> ();
		cam.enabled = true;
		camObj.SetActive (true);
		camObj = cameras [1];
		cam = camObj.GetComponent<Camera> ();
		cam.enabled = false;
		camObj.SetActive (false);
		state = State.Bot;

		if (firstMeter) {
			firstMeter = false;
			tutostage = 10;
		}

		PlayAudio (3);
		score += 500;
		countdown += 5.0f;
	}

	public void firstTuto(int t){
		paused = true;

		if (t == 0) {
			if (tutostage == 0) {
				text.text = "A seta amarela aponta casas com\nirregularidades nos medidores de água.";
			} else if (tutostage == 1) {
				text.text = "Dirija-se até lá e pressione\nqualquer botão para entrar.";
			} else if (tutostage == 2) {
				paused = false;
				text.text = "";
			}
		} else if (t == 1) {
			if (tutostage == 0) {
				text.text = "Um cano da rua estourou.\nEncontre-o e pressione qualquer botão para\nconsertá-lo.";
			} else if (tutostage == 1) {
				paused = false;
				text.text = "";
			}
		} else if (t == 2) {
			image.enabled = true;
			if (MeterBoxController.controller.state == MeterBoxController.State.Closed) {
				text.text = "Utilize o analógico esquerdo\npara abrir a tampa do medidor.";
				image.sprite = sprites[0];
			} else if (MeterBoxController.controller.state == MeterBoxController.State.Pointers) {
				text.text = "Utilize os analógicos para manter\nos ponteiros na área destacada.";
				image.sprite = sprites[1]; 
			} else if (MeterBoxController.controller.state == MeterBoxController.State.Open) {
				text.text = "Utilize o analógico esquerdo\npara fechar a tampa do medidor.";
				image.sprite = sprites[2];
			}
			if (tutostage == 10) {
				image.enabled = false;
				paused = false;
				text.text = "";
			}
		}
	}
}
