using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

public class mainControler : MonoBehaviour {

	private int[] bigBoard = new int[9];
	private GameObject[] bigBoardsObjects = new GameObject[9];
	private int[,] smallBoards = new int[9, 9];//first number is the board second is spot in board
	private GameObject[,] smallBoardsObjects = new GameObject[9, 9];

	private int[,] pieceLocationsX = new int[9, 9];//first number is the board second is spot in board
	private int[,] pieceLocationsY = new int[9, 9];//first number is the board second is spot in board

	bool gameOver = false;
	private Vector3 mouseSpot;

	private int[] results = { 0, 0};

	private int turnColor = 1;

	int currentActiveBoard = 0;
	int lastActiveBoard = 0;

	public Camera c;

	Color tile1Color = new Color32(242, 27, 63, 255);
	Color tile2Color = new Color32(238, 232, 44, 255);

	Color activeBoardColor = new Color32(127, 35, 255, 255);
	Color inactiveBoardColor = new Color32(255, 255, 255, 255);

	public GameObject peice;
	public GameObject turnIndicator;

	void Start() {
		for (int i = 0; i < 3; i++) {
			bigBoard[i] = 0;
			for (int j = 0; j < 3; j++) {
				smallBoards[i, j] = 0;
			}
		}

		int spotX = -15;
		int spotY = 15;
		for (int i = 0; i < 9; i++) {//all boards
			for (int j = 0; j < 9; j++) {//all spots on a board

				//Debug.Log(spotX + " " + spotY);
				pieceLocationsX[i, j] = spotX;
				pieceLocationsY[i, j] = spotY;

				if ((j + 1) % 3 == 0) {
					spotX -= 2 * 3;
					spotY -= 3;
				}
				else {
					spotX += 3;
				}
			}

			if ((i + 1) % 3 == 0) {
				spotX = -15;
				spotY -= 3;
			}
			else {
				spotX += 12;
				spotY += 9;
			}
		}
		//Debug.Log("Setup Complete");
	}

	// Update is called once per frame
	void Update() {
		//Debug.Log("update");
		mouseSpot = c.ScreenToWorldPoint(Input.mousePosition);

		if (gameOver == false) {
			if (Input.GetMouseButtonUp(0)) {
				getPlaceSpot(mouseSpot);
				if (attemptToPlace(mouseSpot, currentActiveBoard, turnColor) == true) {
					results = checkForMiniWin(lastActiveBoard, turnColor);
					if (results[0] > 0) {
						//Debug.Log("Mini Win " + results[0]+ " " + results[1]);
						//Debug.Log("Mini Board (" + (lastActiveBoard + 1) + ")");
						bigBoard[lastActiveBoard] = turnColor;
						Tilemap myTM = GameObject.Find("Mini Board (" + (lastActiveBoard+1) + ")").GetComponent<Tilemap>();
						if (turnColor == 1) {
							myTM.color = tile1Color;
						}
						else {
							myTM.color = tile2Color;
						}
						results = checkForLargeWin(turnColor);
						if (results[0] > 0) {
							Debug.Log("Winner");
						}
					}
					turnColor = changeColor(turnColor);
				}
			}
		}
	}


	public bool attemptToPlace(Vector3 mcl, int cab, int tc) {
		int spot = getPlaceSpot(mouseSpot);
		if (spot == -999) {
			return false;
		}
		if (smallBoards[cab, spot] == 0) {
			GameObject clone;
			clone = Instantiate(peice, new Vector3(pieceLocationsX[cab, spot], pieceLocationsY[cab, spot], 0), Quaternion.identity);
			//audioSource.PlayOneShot(hitting, 1.0f);
			//Debug.Log("played sound");
			smallBoardsObjects[cab, spot] = clone;
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
			smallBoards[cab, spot] = tc;

			lastActiveBoard = currentActiveBoard;
			currentActiveBoard = spot;

			Tilemap myTMlab = GameObject.Find("Mini Board (" + (lastActiveBoard + 1) + ")").GetComponent<Tilemap>();
			myTMlab.color = inactiveBoardColor;
			Tilemap myTMcab = GameObject.Find("Mini Board (" + (currentActiveBoard + 1) + ")").GetComponent<Tilemap>();
			myTMcab.color = activeBoardColor;


			return true;
		}
		return false;
	}

	int getPlaceSpot(Vector3 mcl) {
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				if (mcl.x >= 6 * j + 1.5 - 2.5 && mcl.x <= 6 * j + 1.5 + 2.5) {
					if (mcl.y >= -6 * i + (-24) - 2.5 && mcl.y <= -6 * i + (-24) + 2.5) {
						//Debug.Log(i * 3 + j);
						return (i*3 + j);
					}
				}
				//Debug.Log(i + " " + j);
			}
		}
		//Debug.Log("That is not a valid spot to place a piece");
		return -999;
	}

	private int[] checkForMiniWin(int lab, int tc) {//0,0,0 equals loss //1=vertical 2=horozontal 3=diagnagal b-t 4=diagnol t-b// then start spot x then y
		for (int i = 0; i < 3; i++) {
			if (smallBoards[lab, i * 3] == tc && smallBoards[lab, i * 3 + 1] == tc && smallBoards[lab, i * 3 + 2] == tc) {
				int[] winningLocation = { 1, i * 3 };
				return winningLocation;
			}

			if (smallBoards[lab, i] == tc && smallBoards[lab, i+3] == tc && smallBoards[lab, i+6] == tc) {
				int[] winningLocation = { 2, i };
				return winningLocation;
			}

		}

		if (smallBoards[lab, 0] == tc && smallBoards[lab, 4] == tc && smallBoards[lab, 8] == tc) {
			int[] winningLocation = { 3, 0};
			return winningLocation;
		}

		if (smallBoards[lab, 6] == tc && smallBoards[lab, 4] == tc && smallBoards[lab, 2] == tc) {
			int[] winningLocation = { 4, 6};
			return winningLocation;
		}


		//draw
		int countDraw = 0;
		for (int i = 0; i < 9; i++) {
				if (smallBoards[lab, i] != 0) {
					countDraw++;
				}
		}
		if (countDraw >= 7 * 6) {
			int[] winningLocation = { 5, 0};
			return winningLocation;
		}

		int[] wl = { 0, 0};
		return wl;
	}

	private int[] checkForLargeWin(int tc) {//0,0,0 equals loss //1=vertical 2=horozontal 3=diagnagal b-t 4=diagnol t-b// then start spot x then y
		for (int i = 0; i < 3; i++) {
			if (bigBoard[i * 3] == tc && bigBoard[i * 3 + 1] == tc && bigBoard[i * 3 + 2] == tc) {
				int[] winningLocation = { 1, i * 3 };
				return winningLocation;
			}

			if (bigBoard[i] == tc && bigBoard[i + 3] == tc && bigBoard[i + 6] == tc) {
				int[] winningLocation = { 2, i };
				return winningLocation;
			}

		}

		if (bigBoard[0] == tc && bigBoard[4] == tc && bigBoard[8] == tc) {
			int[] winningLocation = { 3, 0 };
			return winningLocation;
		}

		if (bigBoard[6] == tc && bigBoard[4] == tc && bigBoard[2] == tc) {
			int[] winningLocation = { 4, 6 };
			return winningLocation;
		}


		//draw
		int countDraw = 0;
		for (int i = 0; i < 9; i++) {
			if (bigBoard[i] != 0) {
				countDraw++;
			}
		}
		if (countDraw >= 7 * 6) {
			int[] winningLocation = { 5, 0 };
			return winningLocation;
		}

		int[] wl = { 0, 0 };
		return wl;
	}

	private int changeColor(int tc) {
		return (tc * -1);
	}
}
