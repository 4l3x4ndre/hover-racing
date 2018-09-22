using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// This script is use to move the player's arrow
/// above the ship - use with minimap

public class ArrowBehaviour : MonoBehaviour {

	public Transform follow;

	void Update () {
		if (follow != null) {
			transform.position = new Vector3 (follow.position.x,
				transform.position.y,
				follow.position.z);
		}

	}
}
