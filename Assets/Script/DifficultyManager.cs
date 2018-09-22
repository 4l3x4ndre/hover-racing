using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyManager : MonoBehaviour {

	public bool isInGame;
	public GameObject beginnerAI;
	public GameObject competentAI;
	public GameObject expertAI;

	public Dropdown drop;

	int difficultyLevel;


	void Awake () {
		SetDifficultyLevel ();
		if (isInGame) {

			if (difficultyLevel == 0) {
				beginnerAI.SetActive (true);
			} else if (difficultyLevel == 1) {
				competentAI.SetActive (true);
			} else if (difficultyLevel == 2) {
				expertAI.SetActive (true);
			}

		} else {
			ActualiseDropDownValue ();
		}
	}

	public void ActualiseDropDownValue() {
		SetDifficultyLevel ();
		drop.value = difficultyLevel;
	}

	void SetDifficultyLevel() {
		difficultyLevel = PlayerPrefs.GetInt ("difficultyLevel", 0);
	}

	public void DropDownValueChange() {
		PlayerPrefs.SetInt ("difficultyLevel", drop.value);
	}
}
