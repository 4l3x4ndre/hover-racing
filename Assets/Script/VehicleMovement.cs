using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovement : MonoBehaviour {

	public float speed;
	public GameObject followCamera;
	public GameObject internCamera;
	public GameObject frontCanvas;
	public GameObject backCanvas;

	[Header("Drive Settings")]
	public float driveForce = 17f;
	public float slowingVelFactor = .99f;
	public float brakingVelFactor = .95f;
	public float angleOfRoll = 30f;
	public float gyroFacteur = 1.5f;
	float AxisGyro;
	int isGyroActive;


	[Header("Boost Settings")]
	public float boostForce;
	public float boostTime;
	private float oldDriveForce;

	[Header("Hover Settings")]
	public float hoverHeight = 1.5f;
	public float maxGroundDist = 5f;
	public float hoverForce = 300f;
	public LayerMask whatIsGround;
	public PIDController hoverPID;

	[Header("Physics Settings")]
	public Transform shipBody;
	public float terminalVelocity = 100f;
	public float hoverGravity = 20f;
	public float fallGravity = 80f;

	Rigidbody rigidBody;
	PlayerInput input;
	float drag;
	bool isOnGround;

	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
		input = GetComponent<PlayerInput> ();

		// ship's drag value
		drag = driveForce / terminalVelocity;

		oldDriveForce = driveForce;

		isGyroActive = PlayerPrefs.GetInt ("gyro", 0);
	}
	

	void FixedUpdate () {
		
		// how much of ship's velocity is in the forward direction
		speed = Vector3.Dot (rigidBody.velocity, transform.forward);

		CalculateHover ();
		CalculatePropulsion ();

		GameObject.FindObjectOfType<GameManager> ().playerSpeed = Mathf.FloorToInt(speed);
		if (GameObject.FindObjectOfType<GameManager> ().hasFinish && speed > 0) {
			speed = 0;
			driveForce = 0;
			input.thruster = 0;
			input.rudder = 0;
			input.isBraking = false;
		}
	}

	void Update() {
		// Change camera
		if (Input.GetKeyDown(KeyCode.C)) {
			if (internCamera.activeSelf) {
				internCamera.SetActive (false);
				backCanvas.SetActive (true);
				frontCanvas.SetActive (false);
			} else {
				internCamera.SetActive (true);
				frontCanvas.SetActive (true);
				backCanvas.SetActive (false);
			}

		}

		AxisGyro = input.rudder;
		//print (AxisGyro);
	}

	void CalculateHover() {
		Vector3 groundNormal;

		Ray ray = new Ray (transform.position, -transform.up);
		RaycastHit hitInfo;

		isOnGround = Physics.Raycast (ray, out hitInfo, maxGroundDist, whatIsGround);

		if (isOnGround) {

			float height = hitInfo.distance;
			
			groundNormal = hitInfo.normal.normalized;

			float forcePercent = hoverPID.Seek (hoverHeight, height);

			Vector3 force = groundNormal * hoverForce * forcePercent;

			Vector3 gravity = -groundNormal * hoverGravity * height;

			rigidBody.AddForce (force, ForceMode.Acceleration);
			rigidBody.AddForce (gravity, ForceMode.Acceleration);

		} else {

			groundNormal = Vector3.up;

			Vector3 gravity = -groundNormal * fallGravity;
			rigidBody.AddForce (gravity, ForceMode.Acceleration);

		}

		Vector3 projection = Vector3.ProjectOnPlane (transform.forward, groundNormal);
		Quaternion rotation = Quaternion.LookRotation (projection, groundNormal);

		rigidBody.MoveRotation (Quaternion.Lerp (rigidBody.rotation, rotation, Time.deltaTime * 10f));

		float angle;

		if (isGyroActive == 0) { // With keys 
			angle = angleOfRoll * -input.rudder;
		} else {// With gyro
			angle = angleOfRoll * -AxisGyro;
		}

		Quaternion bodyRotation = transform.rotation * Quaternion.Euler (0f, 0f, angle);

		shipBody.rotation = Quaternion.Lerp (shipBody.rotation, bodyRotation, Time.deltaTime * 10f);
	}

	void CalculatePropulsion() {
		
		float rotationTorque;

		if (isGyroActive == 0) { // With keys 
			rotationTorque = input.rudder - rigidBody.angularVelocity.y;
		} else {// With gyro
			rotationTorque = AxisGyro - rigidBody.angularVelocity.y;
		}



		rigidBody.AddRelativeTorque (0f, rotationTorque, 0f, ForceMode.VelocityChange);

		float sidewaysSpeed = Vector3.Dot (rigidBody.velocity, transform.right);

		Vector3 sideFriction = -transform.right * (sidewaysSpeed / Time.fixedDeltaTime);

		rigidBody.AddForce (sideFriction, ForceMode.Acceleration);

		if (input.thruster <= 0f)
			rigidBody.velocity *= slowingVelFactor;

		if (!isOnGround)
			return;

		if (input.isBraking)
			rigidBody.velocity *= brakingVelFactor;

		float propulsion = driveForce * input.thruster - drag * Mathf.Clamp (speed, 0f, terminalVelocity);
		rigidBody.AddForce (transform.forward * propulsion, ForceMode.Acceleration);
	}

	void OnCollisionStay(Collision collision) {
		if (collision.gameObject.layer == LayerMask.NameToLayer ("Wall")) {
			Vector3 upwardForceFromCollision = Vector3.Dot (collision.impulse, transform.up) * transform.up;
			rigidBody.AddForce (-upwardForceFromCollision, ForceMode.Impulse);
		}
	}

	public float GetSpeedPercentage() {
		return rigidBody.velocity.magnitude / terminalVelocity;
	}

	public void SpeedBoost() {
		StartCoroutine (ApplyBoost ());
	}

	IEnumerator ApplyBoost() {
		driveForce += boostForce;
		yield return new WaitForSeconds (boostTime);
		driveForce = oldDriveForce;
		//speed -= driveForce / 4;
	}
}
