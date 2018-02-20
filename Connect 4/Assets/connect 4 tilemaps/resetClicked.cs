using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetClicked : MonoBehaviour {

	public bool powerLevel;//true for reset at any time false for reset only at game over
	public connect4Controle c4c;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown() {
		//Debug.Log("Sprite Clicked");
		c4c.resetClicked(powerLevel);
	}
}
