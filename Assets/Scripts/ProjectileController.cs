using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {
	public float speed = 3.0f;
	public float lifeTime = 4.0f;
	public float maxLightIntensity = 8.0f;
	public float fadeRate = 0.1f;
	public float minLightIntensity = 5.0f;
	public float startAngularVelocity = -1.0f;
	public float slowdownRate = 0.5f;
	
	private Rigidbody2D rb;
	private GameObject player;


	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");

		// Set the trajectory of the projectile to mouse position
		rb = GetComponent<Rigidbody2D> ();

		// Get the distance between the mouse and the player,
		// then normalize because we use it to determine projectile trajectory

		// Need to find camera to convert screen space to world space
		// That allows us to find position of the mouse wrt to the player in the world space

		if (Camera.main != null) {
			// Get the mouse position, then convert to world space
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			Vector2 distance = new Vector2(mousePosition.x - player.transform.position.x,
			                               mousePosition.y - player.transform.position.y
			                               );

			// Ensures that the max magnitude of the vector is 1
			// With this implementation, the further away the mouse is, the faster the projectile
			distance = Vector2.ClampMagnitude(distance, 1.0f);

			rb.velocity = new Vector2 (distance.x, distance.y) * speed;
			
			// Give angular momentum to the projectile about the z-axis
			// The spin direction changes based on trajectory
			rb.angularVelocity = startAngularVelocity * Mathf.Sign (distance.x);

		} else {
			Debug.Log ("Main Camera is missing");
		}
	}


	void OnTriggerEnter2D(Collider2D other) {
		// Makes the light stick to the surface it lands on
		if (other.tag == "Platform") {
			rb.gravityScale = 0.0f;
			rb.velocity = Vector2.zero;
		}
	}
}
