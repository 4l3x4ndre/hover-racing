using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {

	public GameObject pausePanel;
	public GameObject pauseButton;
	public GameObject[] joysticks;

	public void Update() {
		if (GameObject.FindObjectOfType<PlayerInput> () == null) {
			pauseButton.SetActive (false);
			foreach (GameObject g in joysticks) {
				g.SetActive (false);
			}
		}
	}

	public void Replay() {
		Time.timeScale = 1;
		SceneManager.LoadScene ("game");
	}

	public void PauseGame() {
		pausePanel.SetActive (!pausePanel.activeSelf);
		pauseButton.SetActive (!pauseButton.activeSelf);
		if (Time.timeScale == 0) {
			Time.timeScale = 1;
			GameObject.FindObjectOfType<PlayerInput> ().enabled = true;
		} else {
			Time.timeScale = 0;
			GameObject.FindObjectOfType<PlayerInput> ().enabled = false;
		}
	}

	public void Menu() {
		Time.timeScale = 1;
		SceneManager.LoadScene ("Menu");
	}
}
