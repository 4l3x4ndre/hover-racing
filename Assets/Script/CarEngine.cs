using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour {

	public Transform path;
	public float maxSteerAngle = 45.0f;
	public WheelCollider wheelFL;
	public  WheelCollider wheelFR;
	public float clothDistance;
	public float normalDriveForce;
	public float clothDriveForce;

	private List<Transform> nodes;
	private int currentNode = 0;
	private bool isClothOfWaypoint = false;

	private void Start () {
		Transform[] pathTransform = path.GetComponentsInChildren<Transform> ();
		nodes = new List<Transform> ();

		for (int i = 0; i < pathTransform.Length; i++) {
			if (pathTransform [i] != path.transform) {
				nodes.Add (pathTransform [i]);
			}
		}
	}
	

	private void FixedUpdate () {
		ApplySteer ();
		Drive ();
		CheckWaypointDistance ();
	}

	private void ApplySteer() {
		Vector3 relativeVector = transform.InverseTransformPoint (nodes [currentNode].position);
		//relativeVector /= relativeVector.magnitude;
		float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
		wheelFL.steerAngle = newSteer;
		wheelFR.steerAngle = newSteer;
	}

	private void Drive() {
		if (!isClothOfWaypoint) {
			wheelFL.motorTorque = normalDriveForce;
			wheelFR.motorTorque = normalDriveForce;
		} else {
			wheelFL.motorTorque = clothDriveForce;
			wheelFR.motorTorque = clothDriveForce;
		}
	}

	private void CheckWaypointDistance () {
		if (Vector3.Distance (transform.position, nodes [currentNode].position) < 0.5f) {
			if (currentNode == nodes.Count - 1) {
				currentNode = 0;
			} else {
				currentNode++;
			}
		}
		if (Vector3.Distance (transform.position, nodes [currentNode].position) < clothDistance) {
			isClothOfWaypoint = true;
		} else {
			isClothOfWaypoint = false;
		}
	}
}
