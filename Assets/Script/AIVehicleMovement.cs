using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVehicleMovement : MonoBehaviour {


	[Header("Speed Settings")]
	public float speed;
	public float maxSpeed;
	public float maxClothSpeed;
	public float turnSpeed;

	[Header("State Settings")]
	public bool isBraking = false;
	public bool isClothOfWaypoint = false;
	public bool isAccelerating = false;
	public bool isBoosting = false;
	public bool isFrontOfSomething = false;

	[Header("Sensor Settings")]
	public float obstalesDetection;
	public Transform frontSensor;
	public Transform sensorFL;
	public Transform sensorFR;

	[Header("Drive Settings")]
	public float driveForce = 17f;
	public float turnDriveForce;
	public float backFriveForce;
	public float currentDriveForce;
	public float slowingVelFactor = .99f;
	public float brakingVelFactor = .95f;
	public float angleOfRoll = 30f;

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


	[Header("Path Settings")]
	public Transform path;
	private List<Transform> nodes;
	public int currentNode = 0;
	public float clothDistance;

	[Header("FX Settings")]
	public ParticleSystem wallGrind;		//The wall grind particles
	public ParticleSystem thrusterL;		//The thrust effect (left)
	public ParticleSystem thrusterR;		//The thrust effect (right)
	public TrailRenderer trailL;			//The trails effect (left)
	public TrailRenderer trailR;			//The trails effect (right)
	AudioSource engineAudio;				
	public float engineMinVol = 0f;			//The minimum volume of the engine
	public float engineMaxVol = .6f;		//The maximum volume of the engine
	public float engineMinPitch = .3f;		//The minimum pitch of the engine
	public float engineMaxPitch = .8f;		//The maximum pitch of the engine

	Rigidbody rigidBody;
	float drag;
	bool isOnGround;
	float rubber;
	float angle;
	float dist;

	/// RACE SETTINGS \\\
	GameManager gm;
	[HideInInspector] public int lap;
	[HideInInspector] int maxLap;
	[HideInInspector] public bool hasPassSecondGate;


	void Start () {
		rigidBody = GetComponent<Rigidbody> ();

		// ship's drag value
		drag = currentDriveForce / terminalVelocity;

		oldDriveForce = driveForce;

		Transform[] pathTransform = path.GetComponentsInChildren<Transform> ();
		nodes = new List<Transform> ();
		for (int i = 0; i < pathTransform.Length; i++) {
			if (pathTransform [i] != path.transform) {
				nodes.Add (pathTransform [i]);
			}
		}

		//Stop the wall grind particles of they happen to be playing,
		//stop trails effect, and thruster
		wallGrind.Stop();
		thrusterL.Stop ();
		thrusterR.Stop ();
		trailL.enabled = false;
		trailR.enabled = false;

		//Lap settings
		lap = 1;
		gm = GameObject.FindObjectOfType<GameManager> ();
		maxLap = gm.totalLap;

		engineAudio = GetComponent<AudioSource>();

	}

	void FixedUpdate () {
		if (!gm.playerCanMove)
			return;

		// how much of ship's velocity is in the forward direction
		speed = Vector3.Dot (rigidBody.velocity, transform.forward);

		CalculateHover ();
		CalculatePropulsion ();
		CheckWaypointDistance ();
		if (!isFrontOfSomething) {
			if (isClothOfWaypoint) {
				if (!isBoosting) {
					currentDriveForce = turnDriveForce;
				}
			} else {
				if (!isBoosting) {
					currentDriveForce = driveForce;
				}
			}
		} else {
			currentDriveForce = backFriveForce;
		}


		/*
		if (gm.hasFinish && speed > 0) {
			speed = 0;
			currentDriveForce = 0;
		}
*/
	}

	void Update() {

		if (speed > 0) {
			trailL.enabled = true;
			trailR.enabled = true;
			thrusterL.Play ();
			thrusterR.Play ();
		} else {
			trailL.enabled = false;
			trailR.enabled = false;
			thrusterL.Stop ();
			thrusterR.Stop ();
		}

		//Check lap
		if (lap > maxLap && gm.canChangeSettings) {
			gm.hasFinish = true;
			gm.playerCanMove = false;
			gm.winTextObj.text = gm.looseText;
			gm.canChangeSettings = false;
			gm.StartBackToMenu ();
		}


		isFrontOfSomething = Physics.Raycast (transform.position, frontSensor.forward, obstalesDetection) || 
			Physics.Raycast (transform.position, sensorFL.forward, obstalesDetection) || 
			Physics.Raycast (transform.position, sensorFR.forward, obstalesDetection);


		/// Audio Section \\\
		 //Get the percentage of speed the ship is traveling
		float speedPercent = GetSpeedPercentage();
		/// If we have an audio source for the engine sounds...
		if (engineAudio != null)
		{
			//...modify the volume and pitch based on the speed of the ship
			engineAudio.volume = Mathf.Lerp(engineMinVol, engineMaxVol, speedPercent);
			engineAudio.pitch = Mathf.Lerp(engineMinPitch, engineMaxPitch, speedPercent);
		}


		/// AI Direction Setion \\\
		Vector3 direction = Vector3.zero;
		direction.x = nodes [currentNode].position.x;
		direction.y = transform.position.y;
		direction.z = nodes [currentNode].position.z;

		var targetRotation = Quaternion.LookRotation (direction - transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
		//this.transform.LookAt (direction);

		//Quaternion toRotation = Quaternion.FromToRotation (transform.forward, direction);
		//shipBody.rotation = Quaternion.Lerp (transform.rotation, toRotation, turnSpeed * Time.time);

		/*Debug.DrawRay (transform.position, Vector3.right, Color.green);
		Debug.DrawRay (transform.position, Vector3.left, Color.green);
		/*if (Physics.Raycast (transform.position, Vector3.right, dist)) {
			rubber = -0.5f;
		} else if (Physics.Raycast (transform.position, Vector3.left, dist)) {
			rubber = 0.5f;
		} else {
			rubber = 0;
		}
		if (Physics.Raycast (sensorFR.position, sensorFR.forward, dist)) {
			if (speed > 30) {
				rubber = -2.5f;
			} else {
				rubber = -1f;
			}
		} else {
			if (speed > 30) {
				rubber = 2.5f;
			} else {
				rubber = 1f;
			}
		}*/
		/*} else if (Physics.Raycast (sensorFL.position, sensorFL.forward, dist)) {
			rubber = 0.5f;
		} else {
			rubber = 0;
		}*/

	}

	private void CheckWaypointDistance () {
		if (Vector3.Distance (transform.position, nodes [currentNode].position) < 5.5f) {
			if (currentNode == nodes.Count - 1) {
				currentNode = 0;
			} else {
				currentNode++;
			}
		}
		if (!isFrontOfSomething) {
			if (Vector3.Distance (transform.position, nodes [currentNode].position) < clothDistance) {
				isClothOfWaypoint = true;
				if (!isBoosting) {
					currentDriveForce = turnDriveForce;
				}
			} else {
				isClothOfWaypoint = false;
				if (!isBoosting) {
					currentDriveForce = driveForce;
				}
			}
		} else {
			currentDriveForce = backFriveForce;
		}
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

		//float angle = angleOfRoll * -input.rudder;

		//Quaternion bodyRotation = transform.rotation * Quaternion.Euler (0f, 0f, angle);

		//shipBody.rotation = Quaternion.Lerp (shipBody.rotation, bodyRotation, Time.deltaTime * 10f);
	}

	void CalculatePropulsion() {
		float rotationTorque = rubber - rigidBody.angularVelocity.y;
		rigidBody.AddRelativeTorque (0f, rotationTorque, 0f, ForceMode.VelocityChange);

		float sidewaysSpeed = Vector3.Dot (rigidBody.velocity, transform.right);

		Vector3 sideFriction = -transform.right * (sidewaysSpeed / Time.fixedDeltaTime);

		rigidBody.AddForce (sideFriction, ForceMode.Acceleration);

		/*if (input.thruster <= 0f)
			rigidBody.velocity *= slowingVelFactor;*/

		if (!isOnGround)
			return;

		if (isClothOfWaypoint && speed > maxClothSpeed) {
			rigidBody.velocity *= brakingVelFactor;
			isBraking = true;
		} else {
			isBraking = false;
	}

		float propulsion = currentDriveForce * 1 - drag * Mathf.Clamp (speed, 0f, terminalVelocity);
		if (speed < maxSpeed && ((isClothOfWaypoint && speed < maxClothSpeed) || !isClothOfWaypoint)) {
			isAccelerating = true;
			rigidBody.AddForce (transform.forward * propulsion, ForceMode.Acceleration);
		} else {
			isAccelerating = false;
		}
	}

	//Called then the ship collides with something solid
	void OnCollisionStay(Collision collision) {
		if (collision.gameObject.layer == LayerMask.NameToLayer ("Wall")) {
			Vector3 upwardForceFromCollision = Vector3.Dot (collision.impulse, transform.up) * transform.up;
			rigidBody.AddForce (-upwardForceFromCollision, ForceMode.Impulse);
		}
		
		//If the ship did not collide with a wall then exit
		if (collision.gameObject.layer != LayerMask.NameToLayer("Wall"))
			return;

		//Move the wallgrind particle effect to the point of collision and play it
		wallGrind.transform.position = collision.contacts[0].point;
		wallGrind.Play(true);

	}

	//Called when the ship stops colliding with something solid
	void OnCollisionExit(Collision collision)
	{
		//Stop playing the wallgrind particles
		wallGrind.Stop(true);
	}	

	public float GetSpeedPercentage() {
		return rigidBody.velocity.magnitude / terminalVelocity;
	}

	public void SpeedBoost() {
		StartCoroutine (ApplyBoost ());
	}

	IEnumerator ApplyBoost() {
		currentDriveForce += boostForce;
		isBoosting = true;
		yield return new WaitForSeconds (boostTime);
		currentDriveForce = oldDriveForce;
		isBoosting = false;
	}


}
