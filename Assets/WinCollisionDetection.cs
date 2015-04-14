using UnityEngine;
using System.Collections;

public class WinCollisionDetection : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		InitGame.gameFinished = true;
		InitGame.gameWon = true;
	}
}
