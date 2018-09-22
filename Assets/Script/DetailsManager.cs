using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailsManager : MonoBehaviour {

	public bool isInGame;
	public GameObject terrainObj;
	public GameObject decorHigh;
	public GameObject decorMeduim;

	public Dropdown drop;

	int detailsLevel;


	void Awake () {
		SetDetailsLevel ();
		if (isInGame) {

			if (detailsLevel == 1) {
				terrainObj.SetActive (true);
			} if (detailsLevel == 2) {
				terrainObj.SetActive (true);
				decorMeduim.SetActive (true);
			} else if (detailsLevel == 3) {
				terrainObj.SetActive (true);
				decorMeduim.SetActive (true);
				decorHigh.SetActive (true);
			}

		} else {
			ActualiseDropDownValue ();
		}
	}

	public void ActualiseDropDownValue() {
		SetDetailsLevel ();
		drop.value = detailsLevel;
	}

	void SetDetailsLevel() {
		detailsLevel = PlayerPrefs.GetInt ("detailsLevel", 3);
	}
	
	public void DropDownValueChange() {
		PlayerPrefs.SetInt ("detailsLevel", drop.value);
	}
}
