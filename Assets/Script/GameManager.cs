using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.PostProcessing;

public class GameManager : MonoBehaviour {

	public int totalLap;
	public int lap;
	[HideInInspector] public bool hasPassSecondGate;

	public string winText;
	public string looseText;
	public string finalLapText;

	public float timeBeforeCountDown;

	public AudioClip beepLow;
	public AudioClip beepHigh;

	public float timeShowLapTime;

	public GameObject[] Canvas;

	public int UnlockedLevel;
	public int NbLevelUnlockFinish;

	TextMeshPro lapText;
	TextMeshPro speedText;
	TextMeshProUGUI countDownText;
	[HideInInspector] public TextMeshProUGUI winTextObj;
	TextMeshProUGUI lapTimeText;
	TextMeshProUGUI totalTimeText;

	GameObject backgroundMusic;

	[HideInInspector] public bool hasFinish;
	[HideInInspector] public bool playerCanMove;
	[HideInInspector] public int playerSpeed;
	[HideInInspector] public float LapTimer;
	[HideInInspector] public bool finalLap = false;

	bool startTimeCount;
	float TotalmiliSeconds, Totalseconds, Totalminutes;
	float timer;
	float CurrentmiliSeconds, Currentseconds, Currentminutes;
	bool canContinueLapTimer = true;


	[HideInInspector] public bool canChangeSettings = true;

	void Start () {

		backgroundMusic = GameObject.Find ("Background Music");
		backgroundMusic.SetActive (false);

		lap = 1;
		hasPassSecondGate = false;
		hasFinish = false;
		playerCanMove = false;
		startTimeCount = false;

		lapText = GameObject.Find ("PlayerLapText").GetComponent<TextMeshPro> ();
		speedText = GameObject.Find ("PlayerSpeedText").GetComponent<TextMeshPro> ();
		countDownText = GameObject.Find ("CountDownText").GetComponent<TextMeshProUGUI> ();
		totalTimeText = GameObject.Find ("TotalTime").GetComponent<TextMeshProUGUI> ();
		lapTimeText = GameObject.Find ("LapTime").GetComponent<TextMeshProUGUI> ();
		winTextObj = GameObject.Find ("WinText").GetComponent<TextMeshProUGUI> ();
		winTextObj.text = "";

		StartCoroutine (CountDown ());

		int postProcessingValue = PlayerPrefs.GetInt ("postP", 0);
		if (postProcessingValue == 0) {
			GameObject.Find ("Cruiser Main Camera").GetComponent<PostProcessingBehaviour> ().enabled = true;
		} else {
			GameObject.Find ("Cruiser Main Camera").GetComponent<PostProcessingBehaviour> ().enabled = false;
		}

	}

	IEnumerator CountDown() {

		AudioSource audio = GetComponent<AudioSource> ();
		GameObject.Find ("Circles").SetActive (true);
		//countDownText.gameObject.GetComponent<MeshRenderer>().material.SetFloat ("_ ZTestMode", 4);

		yield return new WaitForSeconds (timeBeforeCountDown);
		audio.PlayOneShot (beepLow);
		countDownText.text = "3";

		yield return new WaitForSeconds (1.0f);
		audio.PlayOneShot (beepLow);
		countDownText.text = "2";

		yield return new WaitForSeconds (1.0f);
		audio.PlayOneShot (beepLow);
		countDownText.text = "1";

		yield return new WaitForSeconds (1.0f);
		audio.PlayOneShot (beepHigh);
		countDownText.text = "GO!";
		countDownText.gameObject.GetComponentInParent<Animator> ().SetBool ("Go", true);
		GameObject.Find ("Circles").SetActive (false);
		backgroundMusic.SetActive (true);

		playerCanMove = true;
		startTimeCount = true;

		yield return new WaitForSeconds (0.85f);
		//countDownText.text = "";

	}
	

	void Update () {

		lapText = GameObject.Find ("PlayerLapText").GetComponent<TextMeshPro> ();
		speedText = GameObject.Find ("PlayerSpeedText").GetComponent<TextMeshPro> ();

		lapText.text = lap.ToString () + " / " + totalLap.ToString ();

		if (playerSpeed >= 0) {
			speedText.text = playerSpeed.ToString ();
		} else {
			speedText.text = "0";
		}

		if (lap > totalLap && canChangeSettings) {

			int ul = PlayerPrefs.GetInt ("unlockLevel", 0);
			UnlockedLevel = PlayerPrefs.GetInt ("unlockLevel", 0);

			if (UnlockedLevel < NbLevelUnlockFinish) {
				PlayerPrefs.SetInt ("unlockLevel", NbLevelUnlockFinish);
			}
			//PlayerPrefs.SetInt ("unlockLevel", ul + 1);
			//print(PlayerPrefs.GetInt("unlockLevel"));

			for (int i = 0; i < Canvas.Length; i++) {
				Canvas [i].SetActive (false);
			}

			lap = totalLap;
			hasFinish = true;
			playerCanMove = false;
			winTextObj.text = winText;
			canChangeSettings = false;
			StartCoroutine (BackToMenu ());
		}

		if (startTimeCount && !hasFinish) {

			/// Total Time \\\
			timer += Time.deltaTime;
			Totalminutes = (int)(timer / 60f);
			Totalseconds = (int)(timer % 60f);
			TotalmiliSeconds = (int)(Time.timeSinceLevelLoad * 100f) % 100;
			totalTimeText.text = Totalminutes.ToString("00") + ":" + Totalseconds.ToString("00") + ":" + TotalmiliSeconds.ToString();

			/// Lap Time \\\
			LapTimer += Time.deltaTime;
			if (canContinueLapTimer) {
				Currentminutes = (int)(LapTimer / 60f);
				Currentseconds = (int)(LapTimer % 60f);
				CurrentmiliSeconds = (int)(Time.timeSinceLevelLoad * 10f) % 10;
				lapTimeText.text = Currentminutes.ToString ("00") + ":" + Currentseconds.ToString ("00") + ":" + CurrentmiliSeconds.ToString ();
			}
		}

		/// Final Lap Show on screen
		if (finalLap) {
			countDownText.text = finalLapText;
		}
	}

	public void EndOfLap() {
		canContinueLapTimer = false;
		LapTimer = 0;
		StartCoroutine (ContinueLapTimer ());
	}

	IEnumerator ContinueLapTimer() {
		yield return new WaitForSeconds (timeShowLapTime);
		canContinueLapTimer = true;
	}

	public void StartBackToMenu() {
		StartCoroutine (BackToMenu ());
	}

	IEnumerator BackToMenu() {
		yield return new WaitForSeconds (2.95f);
		SceneManager.LoadScene ("Menu");
	}
}
