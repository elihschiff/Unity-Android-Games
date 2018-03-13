using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

	private int[,] gameBoard = new int[7, 6];//-1=yellow 1=red (0 or null = blank)
	int count = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



	public int makeMove(int[,] gb) {
		gameBoard = gb;
		int moveSpot = -1;
		int tempSpot;

		tempSpot = checkForAWinningSpot(-1);//Checks for winning move
		if (tempSpot >=0) {
			return tempSpot;
		}

		tempSpot = checkForAWinningSpot(1);//Checks for blocking other team win
		if (tempSpot >= 0) {
			return tempSpot;
		}



		//fallback random move
		moveSpot = Random.Range(0, 7);
		while (!(gameBoard[moveSpot, 5] == 0)) {//this could cause issues if all col are full :) I hope I fix this before the project is done and I do not forget
			moveSpot = Random.Range(0, 7 + 1);
		}
		return moveSpot;
	}


	private int checkForAWinningSpot(int tc){
		for (int j = 0; j < 7; j++) {//j=col(x)
			for (int i = 0; i < 6; i++) {//i=row(y)
				if (gameBoard[j, i] == 0) {//this(i) is the first free spot in the row //if this never equals true that row is full

					//vertical
					if (i > 2) {//enough pieces for a vertical win to even happen
						if (gameBoard[j, i - 1] == tc && gameBoard[j, i - 2] == tc && gameBoard[j, i - 3] == tc) {
							Debug.Log("vertical win or blocking a win");
							return j;
						}
					}

					//horizontal
					count = 0;
					for (int p = 0; p < 7; p++) {
						if (gameBoard[p, i] == tc || j == p) {
							count++;
						}
						else {
							count = 0;
						}

						if (count >= 4) {
							Debug.Log("horizontal win or blocking a win");
							return j;
						}
					}



					//diagnal stuff
					// b-t
					count = 0;
					for (int n = 3; n < 7; n++) {
						for (int p = 0; p < 6 - 3; p++) {

							count = 0;
							for (int a = 0; a < 4; a++) {
								if (gameBoard[n - a, p + a] == tc || (j == n - a && i == p + a)) {
									//Debug.Log(n-a + " " + (p + a));
									//Debug.Log((j == n && i == p));
									count++;
								}
								else {
									count = 0;
								}

								if (count >= 4) {
									Debug.Log("b-t win or blocking a win");
									return j;
								}
							}
						}
					}

					count = 0;
					for (int n = 3; n < 7; n++) {
						for (int p = 3; p < 6; p++) {

							count = 0;
							for (int a = 0; a < 4; a++) {
								if (gameBoard[n - a, p - a] == tc || (j == n - a && i == p - a)) {
									//Debug.Log(n-a + " " + (p + a));
									//Debug.Log((j == n && i == p));
									count++;
								}
								else {
									count = 0;
								}

								if (count >= 4) {
									Debug.Log("t-b win or blocking a win");
									return j;
								}
							}
						}
					}
					break;//this is to stop it from going past the first empty tile in a col
				}
			}
		}

		return -1;
	}


	//rc=7 cc=6
	private int[] checkForWin(int tc) {//0,0,0 equals loss //1=vertical 2=horozontal 3=diagnagal b-t 4=diagnol t-b// then start spot x then y
													   //Debug.Log("checking");
													   //vertical
		int countX = 0;
		for (int i = 0; i < 7; i++) {
			for (int j = 0; j < 6; j++) {
				if (gameBoard[i, j] == tc) {
					countX++;
				}
				else {
					countX = 0;
				}

				if (countX >= 4) {
					Debug.Log("Vertical Win");
					int[] winningLocation = { 1, i, j };
					return winningLocation;
				}
			}
		}

		//horozontal
		int countY = 0;
		for (int i = 0; i < 6; i++) {
			for (int j = 0; j < 7; j++) {
				if (gameBoard[j, i] == tc) {
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
				if (gameBoard[i, j] == tc && gameBoard[i - 1, j + 1] == tc && gameBoard[i - 2, j + 2] == tc && gameBoard[i - 3, j + 3] == tc) {
					int[] winningLocation = { 3, i, j };
					return winningLocation;
				}
			}
		}
		// t-b
		for (int i = 3; i < 7; i++) {
			for (int j = 3; j < 6; j++) {
				if (gameBoard[i, j] == tc && gameBoard[i - 1, j - 1] == tc && gameBoard[i - 2, j - 2] == tc && gameBoard[i - 3, j - 3] == tc) {
					int[] winningLocation = { 4, i, j };
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
