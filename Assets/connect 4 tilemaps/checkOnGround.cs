using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkOnGround : MonoBehaviour {

	public Collider2D groundCollider;
	private Rigidbody2D rb;

	//public AudioClip hitting;
	//AudioSource audioSource;

	public connect4Controle c4c;

	private bool turnHasEnded = false;
	private bool movementStarted = false;
	private bool soundPlayed = false;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		//audioSource = GetComponent<AudioSource>();
		//rb.velocity = new Vector2(rb.velocity.x, 0); // set y velocity to zero
		//rb.AddForce(new Vector2(0, -2000));
	}
	
	// Update is called once per frame
	void Update () {
		if (rb.velocity.magnitude >= .3) {
			movementStarted = true;
		}

		if (groundCollider.isTrigger) {
			//Debug.Log("triggered");

			if (soundPlayed == false) {
				//audioSource.PlayOneShot(hitting, 1.0f);
				soundPlayed = true;
			}

			//Debug.Log(rb.velocity.magnitude);
			if (rb.velocity.magnitude <= .2 && movementStarted == true) {


				this.transform.position = new Vector3(Mathf.Round(this.transform.position.x), Mathf.Round(this.transform.position.y), this.transform.position.z);
				if (turnHasEnded == false) {
					//Debug.Log("done");
					c4c.turnHasFinished();
					turnHasEnded = true;
				}
			}
		}
	}
}
