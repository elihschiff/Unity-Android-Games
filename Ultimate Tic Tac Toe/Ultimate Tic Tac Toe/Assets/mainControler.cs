using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

public class mainControler : MonoBehaviour {

	private int[] bigBoard = new int[9];
	private GameObject[] bigBoardsObjects = new GameObject[9];
	private int[,] smallBoards = new int[9, 9];//first number is the board second is spot in board
	private GameObject[,] smallBoardsObjects = new GameObject[9, 9];

	private GameObject[,] placerObjectsBoard = new GameObject[3, 3];
	private Vector3[,] placerBoardLocation = new Vector3[3, 3];

	private int[,] pieceLocationsX = new int[9, 9];//first number is the board second is spot in board
	private int[,] pieceLocationsY = new int[9, 9];//first number is the board second is spot in board

	private IEnumerator coroutine;

	bool gameOver = false;
	private Vector3 mouseSpot;

	private int[] results = { 0, 0 };

	private int turnColor = 1;

	int currentActiveBoard = -1;
	int lastActiveBoard = 0;

	public Camera c;

	Color tile1Color = new Color32(255, 80, 59, 255);
	Color tile2Color = new Color32(255, 202, 58, 255);

	Color activeBoardColor = new Color32(138, 201, 38, 255);
	Color inactiveBoardColor = new Color32(255, 255, 255, 255);

	Color wintileColor = new Color32(50, 205, 50, 255);

	public Sprite resetIconSprite;
	public Sprite squareSprite;

	public GameObject peice;
	public GameObject turnIndicator;

	int aiTimer = 0;

	void Start() {
		for (int i = 0; i < 9; i++) {
			bigBoard[i] = 0;
			for (int j = 0; j < 9; j++) {
				smallBoards[i, j] = 0;
				smallBoardsObjects[i, j] = null;
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

		float X = 1.5f;
		float Y = -24f;
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				placerBoardLocation[j, i] = new Vector3(X, Y, 0);
				X += 6f;
			}
			X = 1.5f;
			Y -= 6f;
		}

		//easy debug
		//results[0] = 4;
		//results[1] = 2;
		//gameOver = true;
	}

	// Update is called once per frame
	void Update() {
		//Debug.Log("update");
		mouseSpot = c.ScreenToWorldPoint(Input.mousePosition);
		if (Input.GetMouseButtonUp(0)) {
			if (mouseSpot.x >= -12 - 2 && mouseSpot.x <= -12 + 2) {
				if (mouseSpot.y >= -35 - 2 && mouseSpot.y <= -35 + 2) {
					resetClicked(true);
				}
			}
		}


		if (gameOver == false && turnColor == -1) {
			if (Input.GetMouseButtonUp(0)) {
				currentActiveBoard = changeActiveBoard(mouseSpot, currentActiveBoard);
				displayActiveBoard(currentActiveBoard);
				if (currentActiveBoard >= 0) {
					Tilemap myTMcab = GameObject.Find("Mini Board (" + (currentActiveBoard + 1) + ")").GetComponent<Tilemap>();
					myTMcab.color = activeBoardColor;
				}
				getPlaceSpot(mouseSpot);
				if (attemptToPlace(mouseSpot, currentActiveBoard, turnColor) == true) {
					results = checkForMiniWin(lastActiveBoard, turnColor);
					if (results[0] > 0 && results[0] != 5) {
						//Debug.Log("Mini Win " + results[0]+ " " + results[1]);
						//Debug.Log("Mini Board (" + (lastActiveBoard + 1) + ")");
						bigBoard[lastActiveBoard] = turnColor;
						Tilemap myTM = GameObject.Find("Mini Board (" + (lastActiveBoard + 1) + ")").GetComponent<Tilemap>();
						if (turnColor == 1) {
							myTM.color = tile1Color;
						}
						else {
							myTM.color = tile2Color;
						}
						results = checkForLargeWin(turnColor);
						if (results[0] > 0) {
							gameOver = true;
							Tilemap mySpriteRenderer = GameObject.Find("Mini Board (" + (lastActiveBoard + 1) + ")").GetComponent<Tilemap>();
							mySpriteRenderer.color = inactiveBoardColor;
							Debug.Log("Winner");
						}
					}
					else if (results[0] == 5) {
						bigBoard[lastActiveBoard] = 3;
						results = checkForLargeWin(turnColor);
						if (results[0] > 0) {
							gameOver = true;
							Tilemap mySpriteRenderer = GameObject.Find("Mini Board (" + (lastActiveBoard + 1) + ")").GetComponent<Tilemap>();
							mySpriteRenderer.color = inactiveBoardColor;
							Debug.Log("Winner");
						}
					}
					turnColor = changeColor(turnColor);
					aiTimer = 90;
				}
			}
		}
		else if (gameOver == false && turnColor == 1 && aiTimer == 0) {

			mouseSpot = new Vector3(Random.Range(-15.0f, 15.0f), Random.Range(-15.0f, 15.0f), 0);//ai pick board to play on

			if (currentActiveBoard < 0) {
				currentActiveBoard = changeActiveBoard(mouseSpot, currentActiveBoard);
				aiTimer = 90;
			}

			displayActiveBoard(currentActiveBoard);
			if (currentActiveBoard >= 0) {
				Tilemap myTMcab = GameObject.Find("Mini Board (" + (currentActiveBoard + 1) + ")").GetComponent<Tilemap>();
				myTMcab.color = activeBoardColor;
			}
			if (aiTimer == 0) {
				//mouseSpot = new Vector3(Random.Range(0.0f, 15.0f), Random.Range(-22.5f, -37.5f), 0);//ai pick spot to play on
				int highestValue = -999999;
				int spot = 0;
				for (int i = 0; i < 9; i++) {
					if (smallBoards[currentActiveBoard, i] == 0) {

						int[] tempBoard = new int[9];
						for (int j = 0; j < 9; j++) {
							tempBoard[j] = smallBoards[currentActiveBoard, j];
						}

						if (tempBoard[i] == 0) {
							tempBoard[i] = turnColor;
							int spotValue = checkAIMove(tempBoard, turnColor, i);
							print(i + " " + spotValue);
							if (spotValue >= highestValue) {
								highestValue = spotValue;
								spot = i;
								//print("change to " + spot);
							}
						}
					}
				}
				//print(spot);
				mouseSpot = new Vector3(6*(spot%3)+1.5f, -24f-(6*Mathf.Floor(spot/3)), 0);//ai pick spot to play on
				print("mouseSpot "+mouseSpot);
			}
			getPlaceSpot(mouseSpot);
			if (attemptToPlace(mouseSpot, currentActiveBoard, turnColor) == true) {
				results = checkForMiniWin(lastActiveBoard, turnColor);
				if (results[0] > 0 && results[0] != 5) {
					//Debug.Log("Mini Win " + results[0]+ " " + results[1]);
					//Debug.Log("Mini Board (" + (lastActiveBoard + 1) + ")");
					bigBoard[lastActiveBoard] = turnColor;
					Tilemap myTM = GameObject.Find("Mini Board (" + (lastActiveBoard + 1) + ")").GetComponent<Tilemap>();
					if (turnColor == 1) {
						myTM.color = tile1Color;
					}
					else {
						myTM.color = tile2Color;
					}
					results = checkForLargeWin(turnColor);
					if (results[0] > 0) {
						gameOver = true;
						Tilemap mySpriteRenderer = GameObject.Find("Mini Board (" + (lastActiveBoard + 1) + ")").GetComponent<Tilemap>();
						mySpriteRenderer.color = inactiveBoardColor;
						Debug.Log("Winner");
					}
				}
				else if (results[0] == 5) {
					bigBoard[lastActiveBoard] = 3;
					results = checkForLargeWin(turnColor);
					if (results[0] > 0) {
						gameOver = true;
						Tilemap mySpriteRenderer = GameObject.Find("Mini Board (" + (lastActiveBoard + 1) + ")").GetComponent<Tilemap>();
						mySpriteRenderer.color = inactiveBoardColor;
						Debug.Log("Winner");
					}
				}
				turnColor = changeColor(turnColor);
			}
		}
		else {
			changeWinningTileColors(results, (Mathf.PingPong(Time.time, 2) <= 1));
		}
		if (aiTimer > 0) {
			aiTimer--;
		}
	}

	public int checkAIMove(int[] board, int turn, int spot) {
		int moveValue = 1*checkAIWin(board, turn, spot);
		moveValue += -1*checkAIWin(board, turn*-1, spot);
		if (moveValue == 0) {
			int[] newBoard = new int[9];
			System.Array.Copy(board, newBoard, board.Length);
			newBoard[spot] = turn;
			for (int i = 0; i < 9; i++) {
				if (newBoard[i] == 0) {
					moveValue -= checkAIMove(newBoard, turn * -1, i);
				}
			}
		}
		return moveValue;
	}

	public int checkAIWin(int[] board, int turn, int spot) {
		int tc = turn;
		for (int i = 0; i < 3; i++) {
			if (board[i * 3] == tc && board[i * 3 + 1] == tc && board[i * 3 + 2] == tc) {
				return 1;
			}

			if (board[i] == tc && board[i + 3] == tc && board[i + 6] == tc) {
				return 1;
			}

		}

		if (board[0] == tc && board[4] == tc && board[8] == tc) {
			return 1;
		}

		if (board[6] == tc && board[4] == tc && board[2] == tc) {
			return 1;
		}


		//draw
		int countDraw = 0;
		for (int i = 0; i < 9; i++) {
			if (board[i] != 0) {
				countDraw++;
			}
		}
		if (countDraw >= 9) {
			return 0;
		}

		return 0;
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
			for (int i = 0; i < 3; i++) {
				Tilemap mySpriteRenderer = GameObject.Find("Mini Board (" + (results[1] + i + 1) + ")").GetComponent<Tilemap>();
				mySpriteRenderer.color = newColor;
				//homerun
			}
		}
		else if (results[0] == 2) {
			for (int i = 0; i < 3; i++) {
				//Debug.Log((results[1] + (i * 3)));
				Tilemap mySpriteRenderer = GameObject.Find("Mini Board (" + (results[1] + 1 + (i * 3)) + ")").GetComponent<Tilemap>();
				mySpriteRenderer.color = newColor;

			}
		}
		else if (results[0] == 3) {
			for (int i = 0; i < 3; i++) {
				//Debug.Log(results[1] + i + " " + results[2]);
				Tilemap mySpriteRenderer = GameObject.Find("Mini Board (" + ((i * 4) + 1 )+ ")").GetComponent<Tilemap>();
				mySpriteRenderer.color = newColor;
			}
		}
		else if (results[0] == 4) {
			for (int i = 0; i < 3; i++) {
				//Debug.Log(results[1] + i + " " + results[2]);
				Tilemap mySpriteRenderer = GameObject.Find("Mini Board (" + (3 + (i * 2)) + ")").GetComponent<Tilemap>();
				mySpriteRenderer.color = newColor;

			}
		}
		else if (results[0] == 5) {
			//draw
		}
	}


	public bool attemptToPlace(Vector3 mcl, int cab, int tc) {
		int spot = getPlaceSpot(mouseSpot);
		if (spot == -999) {
			return false;
		}

		if (cab == -1) {
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


			displayActiveBoard(spot);

			Tilemap myTMlab = GameObject.Find("Mini Board (" + (lastActiveBoard + 1) + ")").GetComponent<Tilemap>();
			myTMlab.color = inactiveBoardColor;
			if (currentActiveBoard >= 0) {
				Tilemap myTMcab = GameObject.Find("Mini Board (" + (currentActiveBoard + 1) + ")").GetComponent<Tilemap>();
				myTMcab.color = activeBoardColor;
			}


			return true;
		}
		return false;
	}

	private void displayActiveBoard(int newBoard) {
		lastActiveBoard = currentActiveBoard;

		if(newBoard == -1) {
			return;
		}

		if (bigBoard[newBoard] != 0) {
			inactivateBoard();
			return;
		}
		else {
			currentActiveBoard = newBoard;
		}

		GameObject clone;
		for(int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				Destroy(placerObjectsBoard[j,i]);

				if (smallBoards[currentActiveBoard, i * 3 + j] == 1) {
					clone = Instantiate(turnIndicator, placerBoardLocation[j, i], Quaternion.identity);
					SpriteRenderer mySpriteRenderer = clone.GetComponent<SpriteRenderer>();
					placerObjectsBoard[j, i] = clone;
					mySpriteRenderer.color = tile1Color;
				}
				else if(smallBoards[currentActiveBoard, i * 3 + j] == -1) {
					clone = Instantiate(turnIndicator, placerBoardLocation[j, i], Quaternion.identity);
					SpriteRenderer mySpriteRenderer = clone.GetComponent<SpriteRenderer>();
					placerObjectsBoard[j, i] = clone;
					mySpriteRenderer.color = tile2Color;
				}
				else {
					placerObjectsBoard[j, i] = null;
				}
			}
		}




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

	int changeActiveBoard(Vector3 mcl, int cab) {
		if(currentActiveBoard == -1) {
			for (int i = 0; i < 3; i++) {
				for (int j = 0; j < 3; j++) {
					if (mcl.x >= pieceLocationsX[i*3+j,4] - 4 && mcl.x <= pieceLocationsX[i * 3 + j, 4] + 4) {
						if (mcl.y >= pieceLocationsY[i * 3 + j, 4] - 4 && mcl.y <= pieceLocationsY[i * 3 + j, 4] + 4) {
							if (bigBoard[i * 3 + j] == 0) {
								//Debug.Log(i * 3 + j);
								return (i * 3 + j);
							}
						}
					}
					//Debug.Log((pieceLocationsY[i * 3 + j, 4] - 4) + " " + (pieceLocationsY[i * 3 + j, 4] + 4));
				}
			}
		}
		//Debug.Log("That is not a valid spot to place a piece");
		return cab;
	}

	private void inactivateBoard() {
		currentActiveBoard = -1;
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				Destroy(placerObjectsBoard[j, i]);
			}
		}
	}

	private int[] checkForMiniWin(int lab, int tc) {//0,0,0 equals loss //1=vertical 2=horozontal 3=diagnagal b-t 4=diagnol t-b// then start spot x then y
		for (int i = 0; i < 3; i++) {
			if (smallBoards[lab, i * 3] == tc && smallBoards[lab, i * 3 + 1] == tc && smallBoards[lab, i * 3 + 2] == tc) {
				int[] winningLocation = { 1, i * 3 };
				if(currentActiveBoard == lab) {
					inactivateBoard();
				}
				return winningLocation;
			}

			if (smallBoards[lab, i] == tc && smallBoards[lab, i+3] == tc && smallBoards[lab, i+6] == tc) {
				int[] winningLocation = { 2, i };
				if (currentActiveBoard == lab) {
					inactivateBoard();
				}
				return winningLocation;
			}

		}

		if (smallBoards[lab, 0] == tc && smallBoards[lab, 4] == tc && smallBoards[lab, 8] == tc) {
			int[] winningLocation = { 3, 0};
			if (currentActiveBoard == lab) {
				inactivateBoard();
			}
			return winningLocation;
		}

		if (smallBoards[lab, 6] == tc && smallBoards[lab, 4] == tc && smallBoards[lab, 2] == tc) {
			int[] winningLocation = { 4, 6};
			if (currentActiveBoard == lab) {
				inactivateBoard();
			}
			return winningLocation;
		}


		//draw
		int countDraw = 0;
		for (int i = 0; i < 9; i++) {
				if (smallBoards[lab, i] != 0) {
					countDraw++;
				}
		}
		if (countDraw >= 9) {
			int[] winningLocation = { 5, 0};
			if (currentActiveBoard == lab) {
				inactivateBoard();
			}
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
		if (countDraw >= 9) {
			int[] winningLocation = { 5, 0 };
			return winningLocation;
		}
		
		int[] wl = { 0, 0 };
		return wl;
	}

	private int changeColor(int tc) {
		return (tc * -1);
	}


	public void resetClicked(bool pl) {
		if (pl == false) {
			if (gameOver == true) {
				//spinning = 1;
			}
		}
		else {
			gameOver = true;
			//Tilemap myTM = GameObject.Find("Mini Board (" + (lastActiveBoard + 1) + ")").GetComponent<Tilemap>();
			rotateBoard mc;
			for (int i= 0; i < 9; i++) {
				mc = GameObject.Find("Mini Board (" + (i + 1) + ")").GetComponent<rotateBoard>();
				//Debug.Log("Mini Board (" + (lastActiveBoard + 1) + ")");

				for (int j = 0; j < 9; j++) {
					Tilemap myTM = GameObject.Find("Mini Board (" + (j + 1) + ")").GetComponent<Tilemap>();
					myTM.color = inactiveBoardColor;
				}

				mc.startRotating();
				for (int j = 0; j < 9; j++) {
					if (smallBoardsObjects[i, j] != null) {
						smallBoardsObjects[i, j].GetComponent<Rigidbody2D>().simulated = true;
						if (j == 4) {
							smallBoardsObjects[i, j].GetComponent<BoxCollider2D>().enabled = false;
						}
					}
				}
			}

			StartCoroutine(startGame());
		}
	}

	private IEnumerator startGame() {
			yield return new WaitForSeconds(5);
		//print("WaitAndPrint " + Time.time);

		rotateBoard mc;
		for (int i = 0; i < 9; i++) {
			mc = GameObject.Find("Mini Board (" + (i + 1) + ")").GetComponent<rotateBoard>();
			//Debug.Log("Mini Board (" + (lastActiveBoard + 1) + ")");
			mc.stopRotating();
		}
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				Destroy(placerObjectsBoard[i, j]);
				placerObjectsBoard[i, j] = null;
			}
		}

		for (int i = 0; i < 9; i++) {
			bigBoard[i] = 0;
			Destroy(bigBoardsObjects[i]);
			bigBoardsObjects[i] = null;
			for (int j = 0; j < 9; j++) {
				smallBoards[i, j] = 0;
				Destroy(smallBoardsObjects[i, j]);
				smallBoardsObjects[i, j] = null;
			}

			Tilemap myTM = GameObject.Find("Mini Board (" + (i + 1) + ")").GetComponent<Tilemap>();
			myTM.color = inactiveBoardColor;
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
		currentActiveBoard = -1;
		gameOver = false;
	}


}
