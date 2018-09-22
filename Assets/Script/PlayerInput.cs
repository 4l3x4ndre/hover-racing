using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerInput : MonoBehaviour {

	public string verticalAxisName = "Vertical";
	public string horizontalAxisName = "Horizontal";
	public string brakingKey = "Brake";
	public Joystick joystick;
	public Button accelerationButton;
	public float gyroSensivity;

	[HideInInspector] public float thruster;
	[HideInInspector] public float rudder;
	public bool isBraking;

	Gyroscope gyro;
	int isGyroActive;

	public GameObject RightBrake;
	public GameObject LeftBrake;
	public GameObject JoystickObj;

	void Start () {
		thruster = rudder = 0f;
		isBraking = false;

		gyro = Input.gyro;
		Input.gyro.enabled = true;
		//joystick = GameObject.Find ("MobileJoystick").GetComponent<Joystick> ();

		isGyroActive = PlayerPrefs.GetInt ("gyro", 0);
		if (isGyroActive == 0) {
			RightBrake.SetActive (true);
			JoystickObj.SetActive (true);
		} else {
			LeftBrake.SetActive (true);
		}
	}

	void Update () {
		if (Input.GetButtonDown ("Cancel") && !Application.isEditor)
			Application.Quit ();



		//print (deviceRotation.x);

		/*if (deviceRotation.x > 0.8f) {
			rudder = -1;
		} else if (deviceRotation.x < 0.8f) {
			rudder = 1;
		}else {
			rudder = 0f;	
		}*/
		//print (gyro.attitude.x);

		//print (gyro.attitude.x);


		if (GameObject.FindObjectOfType<GameManager> ().playerCanMove) {

			/*if (PlayerPrefs.GetInt ("Gyroscope") == 0) {
				rudder = joystick.m_HorizontalVirtualAxis.GetValue;
			} else {*/
			if (Application.platform == RuntimePlatform.Android) {
				/*Quaternion deviceRotation = Input.gyro.attitude;
				rudder = -deviceRotation.x * gyroSensivity;
				if (rudder > 1.5f) {
					rudder = 1.5f;
				} else if (rudder < -1.5f) {
					rudder = -1.5f;
				}*/

				if (isGyroActive == 0) { // With keys 
					rudder = joystick.m_HorizontalVirtualAxis.GetValue;
				} else {// With gyro
					rudder = Input.acceleration.x * 2.5f;
				}
			}else {
		
				thruster = Input.GetAxis (verticalAxisName); // Arrows
				//thruster = joystick.m_VerticalVirtualAxis.GetValue;
				if (isGyroActive == 0) { 
					rudder = Input.GetAxis (horizontalAxisName); // Arrows
				}else {// With gyro
					rudder = Input.acceleration.x * 2.5f;
				}
				//isBraking = Input.GetButton (brakingKey);
			}
		}
	}

	public void ThrusterEnabled() {
		if (GameObject.FindObjectOfType<GameManager> ().playerCanMove) {
			thruster = 1;
		}
	}

	public void ThrusterDisable() {
		if (GameObject.FindObjectOfType<GameManager> ().playerCanMove) {
			thruster = 0;
		}
	}

	public void BrakeEnabled() {
		if (GameObject.FindObjectOfType<GameManager> ().playerCanMove) {
			isBraking = true;
		}
	}

	public void BrakeDisable() {
		if (GameObject.FindObjectOfType<GameManager> ().playerCanMove) {
			isBraking = false;
		}
	}
}