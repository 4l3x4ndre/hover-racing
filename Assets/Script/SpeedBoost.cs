using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour {

	public float speedBoost;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col) {
		if (col.tag == "Player") {
			VehicleMovement vm = col.GetComponent<VehicleMovement> ();
			vm.SpeedBoost ();
		} else if (col.tag == "AI") {
			AIVehicleMovement aivm = col.GetComponent<AIVehicleMovement> ();
			aivm.SpeedBoost ();
		}
	}
}
