using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowsManager : MonoBehaviour {

	public float changeTime = 0.3f;
	public MeshRenderer[] arrows;

	bool change = false;

	void Start () {
		arrows = GetComponentsInChildren<MeshRenderer> ();
		change = false;

		for (int i = 0; i < arrows.Length; i++) {
			if (i != 0) {
				arrows [i].enabled = false;
			}
		}

		StartCoroutine (WaitAndPass ());
	}
	

	void Update () {

		if (change) {
		
			for (int i = 0; i < arrows.Length; i++) {
				if (arrows [i].enabled) {
					arrows [i].enabled = false;

					int nextArrowId = i + 1;
					if (nextArrowId >= arrows.Length) {
						nextArrowId = 0;
					}
					arrows [nextArrowId].enabled = true;

					StartCoroutine(WaitAndPass());
					break;

				}
			}
		
		}

	}

	IEnumerator WaitAndPass() {
		change = false;
		yield return new WaitForSeconds (changeTime);
		change = true;
	}
}
