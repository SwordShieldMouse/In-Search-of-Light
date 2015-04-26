using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {
	public float speed = 3.0f;
	public float lifeTime = 4.0f;
	public float maxLightIntensity = 8.0f;
	public float fadeRate = 0.1f;
	public float minLightIntensity = 4.0f;
	public float startAngularVelocity = -1.0f;
	public float slowdownRate = 0.5f;

	private float spawnTime;
	private Rigidbody rb;
	private Color colour;
	private GameObject player;
	private Light lt;
	private bool isFading;
	private bool slowingDown;

	// Determines if projectile has attached to any surface
	private bool attached;

	void Start () {
		isFading = false;
		attached = false;
		slowingDown = false;
		lt = GetComponent<Light> ();
		colour = GetComponent<Renderer> ().material.color;

		// Set the trajectory of the projectile to mouse position
		rb = GetComponent<Rigidbody> ();
		player = GameObject.FindGameObjectWithTag ("Player");

		// Prevent the projectile from rotating about the x, y axes and messing up the collider
		rb.rotation = Quaternion.Euler (0, 0, rb.rotation.z);

		// Get the distance between the mouse and the player,
		// then normalize because we use it to determine projectile trajectory

		// Need to find camera to convert screen space to world space
		// That allows us to find position of the mouse wrt to the player in the world space

		if (Camera.main != null) {
			// Get the mouse position, then convert to world space
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			Vector3 distance = mousePosition - player.transform.position;
			distance.z = 0;

			// Ensures that the max magnitude of the vector is 1
			// With this implementation, the further away the mouse is, the faster the projectile
			distance = Vector3.ClampMagnitude(distance, 1.0f);

			rb.velocity = new Vector3 (distance.x, distance.y, 0) * speed;

			// Give angular momentum to the projectile about the z-axis
			// The spin direction changes based on trajectory
			rb.angularVelocity = new Vector3(0.0f, 0.0f, startAngularVelocity) * Mathf.Sign (distance.x);

		} else {
			Debug.Log ("Main Camera is missing");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (((Time.time - spawnTime) > lifeTime) && attached) {
			isFading = true;
			GetComponent<ParticleSystem>().Stop();
		}

		if (isFading) {
			colour.a = Mathf.Lerp(colour.a, 0.0f, fadeRate);
			lt.intensity = Mathf.Lerp(lt.intensity, 0.0f, fadeRate);
		} else {
			float theta = (Mathf.Ceil(Time.time) - Time.time) * Mathf.PI;
			lt.intensity = (Mathf.Abs (Mathf.Sin (theta)))
							* maxLightIntensity / (2 * Mathf.PI) + minLightIntensity;
			// We divide by 2pi since Unity gives us the value in radians
		}

		// Deletes the projectile if the light intensity is below a certain threshold
		// and its lifetime has passed
		if (isFading && (lt.intensity <= 0.5f)) {
			Destroy(gameObject);
			player.GetComponent<PlayerController>().UpdateProjectiles();
		}
	}

	void FixedUpdate() {
		// Makes attaching to a platform appear smooth
		if (slowingDown) {
			rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, slowdownRate);
			rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, slowdownRate);
		}
	}

	void OnTriggerEnter(Collider other) {
		// Makes the light stick to the surface it lands on
		if (other.tag == "Platform") {
			rb.useGravity = false;
			attached = true;
			slowingDown = true;
			spawnTime = Time.time;
		}
	}
}
