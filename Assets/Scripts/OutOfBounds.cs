using UnityEngine;
using System.Collections;

public class OutOfBounds : MonoBehaviour {
	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.tag == "Projectile") {
			// Updates projectile count if the object destroyed is a projectile
			Destroy(other.gameObject);
			GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ().UpdateProjectiles ();
		} else if (other.gameObject.tag == "Player") {
			// Restarts the game if the player dies
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
