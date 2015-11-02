using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PreludeScript : MonoBehaviour {
	private string[] texts = new string[5];
	public Text textLabel;
	private int currentText = 0;

	void Awake () {
		texts [0] = "No brasil, quase metade da água tratada\né perdida antes de chegar ao consumidor final.";
		texts [1] = "Entre as principais causas, estão\no rompimento de tubulações...";
		texts [2] = "...ligações clandestinas\ne medidores em situação irregular.";
		texts [3] = "Hidrobot foi criado com o objetivo\nde reduzir a perda de água...";
		texts [4] = "...e você deve ajudá-lo nessa tarefa.";

		textLabel.text = texts [0];
	}
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		textLabel.text = texts [currentText];


		if (Input.anyKeyDown) {
			currentText += 1;
		}
		if(currentText == 5) {
			Application.LoadLevel("MeterBox");
		}
	}
}
