using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Projectile {
	public GameObject gObject;
	public float spawnTime;
	public bool fading;
	public bool slowingDown;

	public Projectile(GameObject proj, float spawn, bool fade = false, bool slow = false) {
		gObject = proj;
		spawnTime = spawn;
		fading = fade;
		slowingDown = slow;
	}
}

public class PlayerController : MonoBehaviour {
	// For the player itself
	public float speed = 5.0f;
	public float jumpSpeed = 2.5f;

	// For the projectile
	public GameObject projectile;
	//public const int projectileLimit = 3;
	public float resourceLimit = 1.0f;
	public float projectileCost = 0.3f;
	public float resourceRefreshRate = 0.1f;
	public float maxLightIntensity = 3.0f;
	public float minLightIntensity = 1.0f;
	public float lightRange = 8.0f;
	public float projectileLifeTime = 4.0f;
	public float fadeRate = 0.5f;

	// For resource bar
	public Slider slider;

	private float currentResource;
	private List<Projectile> projectiles;
	private Rigidbody2D rb;

	
	// Use this for initialization
	void Awake	 () {
		rb = GetComponent<Rigidbody2D> ();
		projectiles = new List<Projectile> ();
		currentResource = resourceLimit;
	}

	void ProjectileCheck() {
		if (Input.GetButtonDown("Fire1") && (currentResource - projectileCost) >= 0) {
			// Spawns a projectile starting at the player,
			// with an offset on the z-axis so that they can be seen
			Projectile p = new Projectile(Instantiate(projectile, new Vector2(transform.position.x, transform.position.y), Quaternion.identity)
			                              as GameObject,
			                              Time.time, false, false);
			// Add lights for projectile
			Light l = p.gObject.AddComponent<Light>();
			l.intensity = maxLightIntensity;
			l.range = lightRange;
			
			projectiles.Add(p);
			currentResource -= projectileCost;
		}
	}

	void UpdateResources() {
		// Update resource bar
		if (currentResource < resourceLimit) {
			currentResource = Mathf.Clamp (currentResource + resourceRefreshRate * Time.deltaTime,
			                               currentResource, resourceLimit);
		}
		slider.value = currentResource;	
	}

	void JumpCheck() {
		if (IsGrounded () && (Input.GetButtonDown ("Jump"))) {
			rb.velocity = new Vector2(
				rb.velocity.x, 
				jumpSpeed
				);
		}
	}

	// Update is called once per frame
	void Update () {
		ProjectileCheck ();

		// Update the projectiles
		UpdateProjectiles ();

		UpdateResources ();

		JumpCheck ();

		// Player light pulses
		float theta = (Mathf.Ceil(Time.time) - Time.time) * Mathf.PI;
		GetComponentInChildren<Light>().intensity = (Mathf.Abs (Mathf.Sin (theta)))
			* maxLightIntensity / (2 * Mathf.PI) + minLightIntensity;
	}
	
	void FixedUpdate() {
		float move = Input.GetAxis ("Horizontal");
		rb.velocity = new Vector2 (
			move * speed,
			rb.velocity.y
			);
	}
	
	bool IsGrounded() {
		return Physics2D.Raycast(rb.position, -Vector2.up, 
		                         GetComponent<Collider2D>().bounds.extents.y + 0.1f);
	}

	void UpdateProjectiles() {
		// Just in case projectiles go out of bounds, so that we dont reference a null object
		projectiles.RemoveAll(x => x.gObject == null);

		foreach (Projectile p in projectiles) {
			if (Time.time > p.spawnTime + projectileLifeTime) {
				p.fading = true;
			}
			// Lighting
			Light light = p.gObject.GetComponent<Light>();
			if (p.fading) {
				light.intensity =  Mathf.Lerp(light.intensity, 0.0f, fadeRate);
				light.range = Mathf.Lerp(light.range, 0.0f, fadeRate);
				//iLight.intensity = Mathf.Lerp(iLight.intensity, 0.0f, fadeRate);
			} else {
				float theta = (Mathf.Ceil(Time.time) - Time.time) * Mathf.PI;
				light.intensity = (Mathf.Abs (Mathf.Sin (theta)))
					* maxLightIntensity / (2 * Mathf.PI) + minLightIntensity;
				// We divide by 2pi since Unity gives us the value in radians
			}
			
			// Deletes the projectile if the light intensity is below a certain threshold
			// and its lifetime has passed
			if (p.fading && (light.intensity <= 0.5f)) {
				Destroy(p.gObject);
			}
		}
		// Now remove the objects that were destroyed
		projectiles.RemoveAll(x => x.gObject == null);
	}
}
