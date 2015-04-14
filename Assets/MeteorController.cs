using UnityEngine;
using System.Collections;

public class MeteorController : MonoBehaviour {

	public float meteorSize;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Mathf.Abs(this.transform.position.x - InitGame.spaceship.transform.position.x) > 50
		    || Mathf.Abs(this.transform.position.y - InitGame.spaceship.transform.position.y) > 25
		    || this.transform.position.z < InitGame.spaceship.transform.position.z - 2) {
			GameObject.DestroyObject (gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		InitGame.gameFinished = true;
		InitGame.gameWon = false;
	}
}
