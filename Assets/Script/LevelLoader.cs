using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

	public GameObject loadingScreen;
	public GameObject levelsPanel;
	public Slider slider;

	public void LoadLevel(int sceneIndex) {
		StartCoroutine (LoadAsynchronously (sceneIndex));
	}

	IEnumerator LoadAsynchronously (int sceneIndex) {
		AsyncOperation operation = SceneManager.LoadSceneAsync (sceneIndex);

		loadingScreen.SetActive (true);
		levelsPanel.SetActive (false);

		while (!operation.isDone) {

			float progress = Mathf.Clamp01 (operation.progress / .9f);
			slider.value = progress;

			yield return null;
		}
	}

}
