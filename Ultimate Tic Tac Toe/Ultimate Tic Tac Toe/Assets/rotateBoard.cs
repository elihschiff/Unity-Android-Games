using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateBoard : MonoBehaviour {

	public Vector3 centerPoint;
	private Vector3 direction = Vector3.forward;

	public int rotating = 0;

	float speed = 100;

	// Use this for initialization
	void Start () {
		//direction = randomizeDirection();
	}
	
	// Update is called once per frame
	void Update () {
		//rotateOnCenter();
		if(rotating == 1) {
			rotateOnCenter(false);
		}else if(rotating == 2){
			rotateOnCenter(true);
		}


	}

	public void rotateOnCenter(bool reset) {
		//Debug.Log(transform.rotation.eulerAngles.z + " " + speed * Time.deltaTime);
		if(rotating == 2 && transform.rotation.eulerAngles.z <= speed * Time.deltaTime){
			transform.RotateAround(centerPoint, direction, transform.rotation.eulerAngles.z);
			Debug.Log("transform.rotation.eulerAngles.z");
			rotating = 0;
		}
		else{
			transform.RotateAround(centerPoint, direction, speed * Time.deltaTime);
		}
	}

	Vector3 randomizeDirection() {
		if (Random.value >= 0.5) {
			return Vector3.forward;
		}
		return Vector3.back;
	}

	public void startRotating() {
		rotating = 1;
	}
	public void stopRotating() {
		rotating = 2;
	}
}
