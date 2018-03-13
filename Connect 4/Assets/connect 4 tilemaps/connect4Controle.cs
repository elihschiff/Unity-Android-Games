using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class connect4Controle : MonoBehaviour {

	public GameObject peice;
	public Camera c;
	private int rowCount = 7;
	private int rowStartSpot = -7;
	private int colCount = 6;
	private int colStartSpot = 14;
	private int turnColor = 1;
	private static bool canMakeTurn = true;


	Vector3 target;
	Vector3 startPosition;

	public Sprite resetIconSprite;
	public Sprite squareSprite;
	//public Collider2D resetCollider;

	private int movingBaseBoard = 0;


	Color tile1Color = new Color32(242, 27, 63, 255);
	Color tile2Color = new Color32(238, 232, 44, 255);
	Color wintileColor = new Color32(50, 205, 50, 255);

	private int[,] gameBoard = new int[7, 6];//-1=yellow 1=red (0 or null = blank)
	private GameObject[,] gameBoardObjects = new GameObject[7, 6];

	//public AudioClip falling;
	//public AudioClip hitting;
	//AudioSource audioSource;

	private Vector3 mouseSpot;

	public GameObject turnIndicator;

	public bool aiActive;


	float currentTime = 0f;
	float timeToMove = 3f;

	private bool gameOver = false;

	private int[] results = {0,0,0};



	public AI aiScript;

	// Use this for initialization
	void Start() {

		target = new Vector3(transform.position.x + 21.01f, transform.position.y, transform.position.z);
		startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

		for (int i = 0; i < rowCount; i++) {
			for (int j = 0; j < colCount; j++) {
				gameBoard[i, j] = 0;
			}
		}
		//audioSource = GetComponent<AudioSource>();

	}

	// Update is called once per frame
	void Update() {
		mouseSpot = c.ScreenToWorldPoint(Input.mousePosition);

		if (gameOver == false) {
			if (aiActive == true && turnColor == -1) {
				if (canMakeTurn == true) {
					int aiSpotPick = aiScript.makeMove(gameBoard);
					Vector3 aiPlaceSpot = new Vector3(aiSpotPick, 0, 0);

					if (attemptToPlace(aiPlaceSpot, rowCount, rowStartSpot, colCount, colStartSpot, turnColor, true) == true) {
						canMakeTurn = false;
						results = checkForWin(rowCount, colCount, turnColor);
						if (results[0] > 0) {
							changeWinningTileColors(results, true);
							gameOver = true;

							SpriteRenderer mySpriteRendererTC = turnIndicator.GetComponent<SpriteRenderer>();
							mySpriteRendererTC.sprite = resetIconSprite;


							//Debug.Break();
						}

						turnColor = changeColor(turnColor);



					}
				}
			}else if (Input.GetMouseButtonUp(0)) {
				//Debug.Log("Pressed left click.");
				if (canMakeTurn == true) {
					if (attemptToPlace(mouseSpot, rowCount, rowStartSpot, colCount, colStartSpot, turnColor, false) == true) {
						canMakeTurn = false;
						results = checkForWin(rowCount, colCount, turnColor);
						if (results[0] > 0) {
							changeWinningTileColors(results, true);
							gameOver = true;

							SpriteRenderer mySpriteRendererTC = turnIndicator.GetComponent<SpriteRenderer>();
							mySpriteRendererTC.sprite = resetIconSprite;


							//Debug.Break();
						}
						
							turnColor = changeColor(turnColor);
						

						
					}
				}
			}
		}
		else {
			changeWinningTileColors(results, (Mathf.PingPong(Time.time, 2) <= 1));

			if (movingBaseBoard == 1) {
				if (currentTime <= timeToMove) {
					//Debug.Log("ct " + currentTime);
					currentTime += Time.deltaTime;
					//Debug.Log("ct " + currentTime);
					this.transform.position = Vector3.Lerp(startPosition, target, currentTime / timeToMove);
				}
				else {
					movingBaseBoard = -10;
					currentTime = 0;
				}
			}
			else if (movingBaseBoard == -10) {
				if (currentTime <= 1) {
					currentTime += Time.deltaTime;
				}
				else {
					movingBaseBoard = -1;
					currentTime = 0;
				}
			}
			else if (movingBaseBoard == -1) {
				if (currentTime <= timeToMove) {
					currentTime += Time.deltaTime;
					this.transform.position = Vector3.Lerp(target, startPosition, currentTime / timeToMove);
				}
				else {
					movingBaseBoard = 0;
					currentTime = 0;
					gameOver = false;


					for (int i = 0; i < rowCount; i++) {
						for (int j = 0; j < colCount; j++) {
							gameBoard[i, j] = 0;
							gameBoardObjects[i, j] = null;
						}
					}


					SpriteRenderer mySpriteRendererTC = turnIndicator.GetComponent<SpriteRenderer>();
					mySpriteRendererTC.sprite = squareSprite;
				}
			}
		}


		//rest game
		//Handheld.Vibrate();
	}


	public void resetClicked(bool pl) {
		if (pl == false) {
			if (gameOver == true) {
				movingBaseBoard = 1;
			}
		}else {
			gameOver = true;
			movingBaseBoard = 1;
		}
	}

	public void turnHasFinished() {
		canMakeTurn = true;
	}

	public void changeWinningTileColors(int[] results, bool colorBool) {

		Color newColor = wintileColor;
		if (colorBool == true) {
			if (turnColor * -1 == 1) {
				newColor = tile1Color;
			}
			else {
				newColor = tile2Color;
			}
		}

		//Debug.Log("color change");
		if (results[0] == 1) {
			for (int i = 0; i < 4; i++) {
				SpriteRenderer mySpriteRenderer = gameBoardObjects[results[1], results[2] - i].GetComponent<SpriteRenderer>();
				mySpriteRenderer.color = newColor;

			}
		} else if (results[0] == 2) {
			for (int i = 0; i < 4; i++) {
				//Debug.Log(results[1] + i + " "+ results[2]);
				SpriteRenderer mySpriteRenderer = gameBoardObjects[results[1] - i, results[2]].GetComponent<SpriteRenderer>();
				mySpriteRenderer.color = newColor;

			}
		} else if (results[0] == 3) {
			for (int i = 0; i < 4; i++) {
				//Debug.Log(results[1] + i + " " + results[2]);
				SpriteRenderer mySpriteRenderer = gameBoardObjects[results[1] - i, results[2] + i].GetComponent<SpriteRenderer>();
				mySpriteRenderer.color = newColor;
			}
		} else if (results[0] == 4) {
			for (int i = 0; i < 4; i++) {
				//Debug.Log(results[1] + i + " " + results[2]);
				SpriteRenderer mySpriteRenderer = gameBoardObjects[results[1] - i, results[2] - i].GetComponent<SpriteRenderer>();
				mySpriteRenderer.color = newColor;

			}
		}else if (results[0] == 5) {
			//draw
		}
	}


	public bool attemptToPlace(Vector3 mcl, int rc, int rss, int cc, int css, int tc, bool isAIPlacing) {//mcl x location is the x location for an ai player
		int spotx;
		if (isAIPlacing == false) {
			spotx = getPlaceSpot(mouseSpot, rowCount, rowStartSpot);
			if (spotx == -999) {
				return false;
			}
		}
		else {
			spotx = (int) mcl.x;
		}
		for (int i = 0; i < cc; i++) {
			if (gameBoard[spotx, i] == 0) {
				GameObject clone;
				clone = Instantiate(peice, new Vector3(3 * spotx + rss, css, 0), Quaternion.identity);
				//audioSource.PlayOneShot(hitting, 1.0f);
				//Debug.Log("played sound");
				gameBoardObjects[spotx, i] = clone;
				SpriteRenderer mySpriteRenderer = clone.GetComponent<SpriteRenderer>();
				SpriteRenderer mySpriteRendererTC = turnIndicator.GetComponent<SpriteRenderer>();
				//Debug.Log(tc);
				if (tc == 1) {
					mySpriteRenderer.color = tile1Color;
					mySpriteRendererTC.color = tile2Color;
				}
				else {
					mySpriteRenderer.color = tile2Color;
					mySpriteRendererTC.color = tile1Color;
				}
				gameBoard[spotx, i] = tc;
				return true;
			}
		}
		return false;
	}

	public int getPlaceSpot(Vector3 mcl, int rc, int rss) {//mcl=mouseClickLocation
		int spot = -999;
		for (int i = 0; i < rc; i++) {
			if (mcl.x >= 3 * i + rss - 1 && mcl.x <= 3 * i + rss + 1) {
				spot = i;
			}
		}
		return spot;
	}

	private int changeColor(int tc) {
		return (tc * -1);

		
	}


	private int[] checkForWin(int rc, int cc, int tc) {//0,0,0 equals loss //1=vertical 2=horozontal 3=diagnagal b-t 4=diagnol t-b// then start spot x then y
		//Debug.Log("checking");
		//vertical
		int countX = 0;
		for (int i = 0; i < rc;  i++){
			for (int j = 0; j < cc; j++) {
				if (gameBoard[i, j] == turnColor) {
					countX++;
				}
				else {
					countX = 0;
				}

				if (countX >= 4) {
					Debug.Log("Vertical Win");
					int[] winningLocation = {1,i,j};
					return winningLocation;
				}
			}
		}

		//horozontal
		int countY = 0;
			for (int i = 0; i < cc; i++) {
				for (int j = 0; j < rc; j++) {
					if (gameBoard[j, i] == turnColor) {
						countY++;
					}
					else {
						countY = 0;
					}

					if (countY >= 4) {
						Debug.Log("horozontal Win");
						int[] winningLocation = { 2, j, i };
						return winningLocation;
					}
				}
			}

		// b-t
		for (int i = 3; i < 7; i++) {
			for (int j = 0; j < 6 - 3; j++) {
				if(gameBoard[i, j] == turnColor && gameBoard[i - 1, j + 1] == turnColor && gameBoard[i - 2, j + 2] == turnColor && gameBoard[i - 3, j + 3] == turnColor) {
					int[] winningLocation = { 3, i, j};
					return winningLocation;
				}
			}
		}
		// t-b
		for (int i = 3; i < 7; i++) {
			for (int j = 3; j < 6; j++) {
				if (gameBoard[i, j] == turnColor && gameBoard[i - 1, j - 1] == turnColor && gameBoard[i - 2, j - 2] == turnColor && gameBoard[i - 3, j - 3] == turnColor) {
					int[] winningLocation = { 4, i, j};
					return winningLocation;
				}
			}
		}


		//draw
		int countDraw = 0;
		for (int i = 0; i < 7; i++) {
			for (int j = 0; j < 6; j++) {
				if (gameBoard[i, j] != 0) {
					countDraw++;
				}
			}
		}
		if (countDraw >= 7 * 6) {
			int[] winningLocation = { 5, 0, 0 };
			return winningLocation;
		}

		int[] wl = { 0, 0, 0 };
		return wl;
	}


}
