using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gates : MonoBehaviour {

	public bool isFirstGate = false;
	public bool isSecondGate = false;
	

	void OnTriggerEnter (Collider col) {

		if (col.tag == "Player") {
			
			GameManager gm = GameObject.FindObjectOfType<GameManager> ();

			if (gm.lap + 1 == gm.totalLap) {
				gm.finalLap = true;
			}

			if (isFirstGate) {

				if (gm.hasPassSecondGate) {
					gm.lap += 1;
					gm.hasPassSecondGate = false;
					gm.EndOfLap ();
				}

			} else if (isSecondGate) {
				gm.hasPassSecondGate = true;
			}
		} else if (col.tag == "AI") {

			AIVehicleMovement aivm = col.GetComponent<AIVehicleMovement> ();
			if (isFirstGate && aivm.hasPassSecondGate) {
				aivm.lap += 1;
				aivm.hasPassSecondGate = false;
			} else if (isSecondGate) {
				aivm.hasPassSecondGate = true;
			}

		}

	}
}
