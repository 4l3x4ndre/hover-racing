using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMotion : MonoBehaviour {

	Rigidbody body;
	Animator anim;

	void Awake () {
		body = GetComponent<Rigidbody> ();
		anim = GetComponent<Animator> ();
	}


	void Update () {
		MovementUpDown ();
		MovementForward ();
		Rotation ();
		ClampingSpeedvalues ();

		body.AddRelativeForce (Vector3.up * currentForce);
		body.rotation = Quaternion.Euler (
			new Vector3(tiltAmountForward, currentYRot, body.rotation.z)
		);
			
	}

	public float currentForce;
	public float normForce;
	public float upForce;
	public float maxUpForce;
	public float downForce;
	public float maxDownForce;
	public float speed;

	public Vector3 velocity;

	void MovementUpDown() {
		/*if (Input.GetKey (KeyCode.I)) {
			if (velocity.y < maxUpForce) {
				currentForce += upForce;
			} else {
				currentForce = 0;
			}
			//body.transform.position += new Vector3 (0f, currentForce * Time.deltaTime, 0f);
		} else if (Input.GetKey (KeyCode.K)) {
			if (velocity.y > maxDownForce) {
				currentForce -= downForce;
			}
		} else if (!Input.GetKey (KeyCode.I) && !Input.GetKey (KeyCode.K)) {
			currentForce = normForce;
		}

		velocity = body.velocity;*/


		Vector3 pos = transform.position;

		if (Input.GetKey (KeyCode.I)) {
			pos.y += upForce * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.K)) {
			pos.y -= downForce * Time.deltaTime;
		}


		transform.position = pos;


	}

	private float movementForwardSpeed = 500.0f;
	private float tiltAmountForward = 0;
	private float tiltVelocityForward;
	void MovementForward() {
		/*Vector2 direction = new Vector2 (Input.GetAxis ("Vertical") * speed, Input.GetAxis ("Horizontal") * speed);
		transform.position += transform.forward * direction.x * Time.deltaTime +
			transform.right * direction.y * Time.deltaTime;*/

		float direction = Input.GetAxis ("Vertical") * speed;
		transform.position += transform.forward * direction * Time.deltaTime;
		if (!Input.GetKey (KeyCode.I) && !Input.GetKey (KeyCode.K)) {
			anim.SetFloat ("ForwardVelocity", direction/speed);
		} else {
			anim.SetFloat ("ForwardVelocity", 0f);
			if (Input.GetKey (KeyCode.I)) {
				anim.SetFloat ("UpDown", 1f);
			} else {
				anim.SetFloat ("UpDown", -1f);
			}
		}

		if (Input.GetKey (KeyCode.I) || Input.GetKey (KeyCode.K)) {
			anim.SetBool ("UpOrDown", true);
		} else {
			anim.SetBool ("UpOrDown", false);
		}


		/*if (Input.GetAxis ("Vertical") != 0) {
			body.AddRelativeForce (Vector3.forward * Input.GetAxis ("Vertical") * movementForwardSpeed);
			tiltAmountForward = Mathf.SmoothDamp (tiltAmountForward, 20 * Input.GetAxis ("Vertical"), ref tiltVelocityForward, 0.1f);
			
		}*/
		/*Vector3 pos = transform.position;

		if (Input.GetKey ("w")) {
			pos.z += speed * Time.deltaTime;
		}
		if (Input.GetKey ("s")) {
			pos.z -= speed * Time.deltaTime;
		}
		if (Input.GetKey ("d")) {
			pos.x += speed * Time.deltaTime;
		}
		if (Input.GetKey ("a")) {
			pos.x -= speed * Time.deltaTime;
		}


		transform.position = pos;*/
	}

	private float wantedYRot;
	private float currentYRot;
	private float RotationAmoutBykeys = 2.5f;
	private float rotationYVelocity;
	void Rotation() {
		if (Input.GetKey (KeyCode.LeftArrow)) {
			wantedYRot -= RotationAmoutBykeys;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			wantedYRot += RotationAmoutBykeys;
		}

		currentYRot = Mathf.SmoothDamp (currentYRot, wantedYRot, ref rotationYVelocity, 0.25f);
		anim.SetFloat ("x-rot", (Input.GetAxis("Horizontal") * speed)/speed);
	}

	private Vector3 velocityTosmoothDampToZero;
	void ClampingSpeedvalues() {
		if (Mathf.Abs (Input.GetAxis ("Vertical")) > 0.2f && Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.2f) {
			body.velocity = Vector3.ClampMagnitude (body.velocity, Mathf.Lerp (body.velocity.magnitude, 10.0f, Time.deltaTime * 5f));
		}
	}
}
