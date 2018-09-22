using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tuto : MonoBehaviour {

	public GameObject tuto;

	int hasCheck;


	void Start () {
		hasCheck = PlayerPrefs.GetInt ("tuto", 0);
	}
	

	void Update () {
		if (hasCheck == 0) {
			tuto.SetActive (true);
		} else {
			SceneManager.LoadScene ("Menu");
		}
	}

	public void HasCheck() {
		PlayerPrefs.SetInt ("tuto", 1);
		SceneManager.LoadScene ("Menu");
	}
}
