using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetClicked : MonoBehaviour {

	public bool powerLevel;//true for reset at any time false for reset only at game over
	public mainControler  mc;

	// Use this for initialization
	void Start () {
		Debug.Log("starting");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown() {
		Debug.Log("Reset Clidked");
		mc.resetClicked(powerLevel);
	}
}
