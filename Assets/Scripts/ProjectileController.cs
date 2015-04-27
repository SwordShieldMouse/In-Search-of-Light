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

	public float spawnTime;
	private Rigidbody2D rb;
	private GameObject player;

	private Light iLight;
	private Light bodyLight;

	public bool isFading;
	public bool slowingDown;

	// Determines if projectile has attached to any surface
	public bool attached;

	void Start () {
		isFading = false;
		attached = false;
		slowingDown = false;

		// Lighting
		iLight = GameObject.FindGameObjectWithTag ("Projectile Illumination").GetComponent<Light> ();
		iLight.intensity = maxLightIntensity;
		bodyLight = GameObject.FindGameObjectWithTag ("Projectile Body").GetComponent<Light> ();

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
	
	// Update is called once per frame
	void Update () {
		if (((Time.time - spawnTime) > lifeTime) && attached) {
			isFading = true;
		} else if (((Time.time - spawnTime) * 1.75f > lifeTime) && attached) {
			// Stop particles a little before lifetime is over to make transition smooth
			GetComponent<ParticleSystem>().enableEmission = false;
		}

		if (isFading) {
			bodyLight.intensity =  Mathf.Lerp(bodyLight.intensity, 0.0f, fadeRate);
			bodyLight.range = Mathf.Lerp(bodyLight.range, 0.0f, fadeRate);
			iLight.intensity = Mathf.Lerp(iLight.intensity, 0.0f, fadeRate);
		} else {
			float theta = (Mathf.Ceil(Time.time) - Time.time) * Mathf.PI;
			iLight.intensity = (Mathf.Abs (Mathf.Sin (theta)))
							* maxLightIntensity / (2 * Mathf.PI) + minLightIntensity;
			// We divide by 2pi since Unity gives us the value in radians
		}

		// Deletes the projectile if the light intensity is below a certain threshold
		// and its lifetime has passed
		if (isFading && (iLight.intensity <= 0.5f)) {
			//Destroy(gameObject);
		}
	}

	void FixedUpdate() {
		// Makes attaching to a platform appear smooth
		if (slowingDown) {
			rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, slowdownRate);
			rb.angularVelocity = Mathf.Lerp(rb.angularVelocity, 0.0f, slowdownRate);
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		// Makes the light stick to the surface it lands on
		if (other.tag == "Platform") {
			rb.gravityScale = 0.0f;
			attached = true;
			slowingDown = true;
			spawnTime = Time.time;
		}
	}

	public IEnumerator ProjectileDeath() {
		
	}
}
