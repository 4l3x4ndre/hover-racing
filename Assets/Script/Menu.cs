using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	[System.Serializable]
	public class ScenesSettings {
		public string levelName;
		public string sceneName;
		public GameObject loadButton;
		public Sprite mapImg;
	}

	public ScenesSettings[] info;

	public GameObject mainPanel;
	public GameObject LevelPanel;
	public GameObject ControlsPanel;
	public GameObject OptionsPanel;

	[Header("Options")]
	public Dropdown postProcessing;
	public bool usGyro = false;
	public Button gyroButton;
	public Dropdown ControlDropDown;
	public Image tutoImg;
	public Sprite tutoJoystick;
	public Sprite tutoGyro;
	public Button optionsButton;


	int unlockLevelNb;
	int gyro;
	int postProcessingValue;

	void Awake () {
		
		foreach (ScenesSettings sc in info) {

			// Initialisation of level name
			foreach (Transform t in sc.loadButton.transform) {

				if (t.GetComponent<TextMeshProUGUI> ()) {
					t.GetComponent<TextMeshProUGUI> ().text = sc.levelName;
				}

			}

			// Put listener;
			/*sc.loadButton.GetComponent<Button> ().onClick.AddListener (delegate {
				LoadScene (sc.sceneName);
			});
*/
		}

		// Check unlock levels - NOT the best way ;-)
		unlockLevelNb = PlayerPrefs.GetInt ("unlockLevel", 0);
		foreach (ScenesSettings sc in info) {
			if (unlockLevelNb == 0) {
				info [1].loadButton.GetComponent<Image> ().sprite = info [1].mapImg;
				info [1].loadButton.GetComponent<Button> ().interactable = true;
			} else if (unlockLevelNb == 1) {
				info [1].loadButton.GetComponent<Image> ().sprite = info [1].mapImg;
				info [1].loadButton.GetComponent<Button> ().interactable = true;
				info [2].loadButton.GetComponent<Image> ().sprite = info [2].mapImg;
				info [2].loadButton.GetComponent<Button> ().interactable = true;
			} else if (unlockLevelNb == 2) {
				info [1].loadButton.GetComponent<Image> ().sprite = info [1].mapImg;
				info [1].loadButton.GetComponent<Button> ().interactable = true;
				info [2].loadButton.GetComponent<Image> ().sprite = info [2].mapImg;
				info [2].loadButton.GetComponent<Button> ().interactable = true;
				info [3].loadButton.GetComponent<Image> ().sprite = info [3].mapImg;
				info [3].loadButton.GetComponent<Button> ().interactable = true;
			} else if (unlockLevelNb == 3) {
				info [1].loadButton.GetComponent<Image> ().sprite = info [1].mapImg;
				info [1].loadButton.GetComponent<Button> ().interactable = true;
				info [2].loadButton.GetComponent<Image> ().sprite = info [2].mapImg;
				info [2].loadButton.GetComponent<Button> ().interactable = true;
				info [3].loadButton.GetComponent<Image> ().sprite = info [3].mapImg;
				info [3].loadButton.GetComponent<Button> ().interactable = true;
				info [0].loadButton.GetComponent<Image> ().sprite = info [0].mapImg;
				info [0].loadButton.GetComponent<Button> ().interactable = true;
			}
		}

		if (usGyro) {
			gyroButton.GetComponentInChildren<Text> ().text = "Click to enable Gyroscope";
		} else {
			gyroButton.GetComponentInChildren<Text> ().text = "Click to disable Gyroscope";
		}

		gyro = PlayerPrefs.GetInt ("gyro", 0);
		ControlDropDown.value = gyro;
		if (gyro == 0) {
			tutoImg.sprite = tutoJoystick;
		}else {
			tutoImg.sprite = tutoGyro;
		}

		postProcessingValue = PlayerPrefs.GetInt ("postP", 0);
		if (postProcessingValue == 0) {
			postProcessing.value = 0;
		} else if (postProcessingValue == 1) {
			postProcessing.value = 1;
		}

	}

	public void ChangeControl() {
		PlayerPrefs.SetInt ("gyro", ControlDropDown.value);
		gyro = PlayerPrefs.GetInt ("gyro", 0);
		if (gyro == 0) {
			tutoImg.sprite = tutoJoystick;
		}else {
			tutoImg.sprite = tutoGyro;
		}
	}

	public void ChangePostProcessingValue() {
		PlayerPrefs.SetInt ("postP", postProcessing.value);
		postProcessingValue = PlayerPrefs.GetInt ("postP", 0);
		if (postProcessingValue == 0) {
			postProcessing.value = 0;
		} else if (postProcessingValue == 1) {
			postProcessingValue = 1;
		}
	}

	void LoadScene (string sceneName) {
		SceneManager.LoadScene (sceneName);
	}

	public void EnableGyroButton() {
		if (usGyro) {
			gyroButton.GetComponentInChildren<Text> ().text = "Click to enable Gyroscope";
			usGyro = false;
			PlayerPrefs.SetInt ("Gyroscope", 0);
		} else {
			gyroButton.GetComponentInChildren<Text> ().text = "Click to disable Gyroscope";
			usGyro = true;
			PlayerPrefs.SetInt ("Gyroscope", 1);
		}
	}
	public void OptionsOrLevels() {
		if (ControlsPanel.activeSelf) {
			ControlsPanel.SetActive (false);
			LevelPanel.SetActive (false);
			mainPanel.SetActive (true);
			optionsButton.GetComponentInChildren<Text> ().text = "Options";
		} else {
			ControlsPanel.SetActive (true);
			LevelPanel.SetActive (false);
			mainPanel.SetActive (false);
			optionsButton.GetComponentInChildren<Text> ().text = "Back";
			if (PlayerPrefs.GetInt ("Gyroscope") == 0) {
				gyroButton.GetComponentInChildren<Text> ().text = "Click to enable Gyroscope";
			} else {
				gyroButton.GetComponentInChildren<Text> ().text = "Click to disable Gyroscope";
			}

		}
	}

	public void SwitchLevels() {
		if (LevelPanel.activeSelf) {
			ControlsPanel.SetActive (false);
			LevelPanel.SetActive (false);
			mainPanel.SetActive (true);
		} else {
			ControlsPanel.SetActive (false);
			LevelPanel.SetActive (true);
			mainPanel.SetActive (false);
		}
	}

	public void Options() {
		OptionsPanel.SetActive (true);
		mainPanel.SetActive (false);
		ControlsPanel.SetActive (false);
		LevelPanel.SetActive (false);
		
	}

	public void BackMainPanel() {
		OptionsPanel.SetActive (false);
		ControlsPanel.SetActive (false);
		LevelPanel.SetActive (false);
		mainPanel.SetActive (true);
	}

	public void ApplicationQuit() {
		Application.Quit ();
	}
}
