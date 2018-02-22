using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateBoard : MonoBehaviour {

	public Vector3 centerPoint;
	private Vector3 direction;

	// Use this for initialization
	void Start () {
		direction = randomizeDirection();
	}
	
	// Update is called once per frame
	void Update () {
		//rotateOnCenter();
	}

	public void rotateOnCenter() {
		transform.RotateAround(centerPoint, direction, 100 * Time.deltaTime);
	}

	Vector3 randomizeDirection() {
		if (Random.value >= 0.5) {
			return Vector3.forward;
		}
		return Vector3.back;
	}
}
