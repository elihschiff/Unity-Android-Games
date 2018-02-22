using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainControler : MonoBehaviour {

	public int[] bigBoard = new int[9];
	public GameObject[] bigBoardsObjects = new GameObject[9];
	public int[,] smallBoards = new int[9,9];//first number is the board second is spot in board
	public GameObject[,] smallBoardsObjects = new GameObject[9, 9];

	public int[,] pieceLocationsX = new int[9, 9];//first number is the board second is spot in board
	public int[,] pieceLocationsY = new int[9, 9];//first number is the board second is spot in board

	bool gameOver = false;

	void Start () {
		for(int i=0; i<3; i++) {
			bigBoard[i] = 0;
			for (int j = 0; j < 3; j++) {
				smallBoards[i, j] = 0;
			}
		}

		int spotX = -15;
		int spotY = 15;
		for (int i = 0; i < 9; i++) {//all boards
			for (int j = 0; j < 9; j++) {//all spots on a board
				
				Debug.Log(spotX + " " + spotY);
				pieceLocationsX[i, j] = spotX;
				pieceLocationsY[i, j] = spotY;

				if ((j+1) % 3 == 0) {
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
	}
	
	// Update is called once per frame
	void Update () {
		if (gameOver == false) {
			if (Input.GetMouseButtonUp(0)) {
					if (attemptToPlace(mouseSpot, rowCount, rowStartSpot, colCount, colStartSpot, turnColor) == true) {
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


	public bool attemptToPlace(Vector3 mcl, int tc) {
		int spotx = getPlaceSpot(mouseSpot, rowCount, rowStartSpot);
		if (spotx == -999) {
			return false;
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

	int getPlaceSpot() {

	}
}
