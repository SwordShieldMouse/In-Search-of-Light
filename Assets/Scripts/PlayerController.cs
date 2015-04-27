using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	public float speed = 5.0f;
	public float jumpSpeed = 2.5f;
	public GameObject projectile;
	public const int projectileLimit = 3;
	public float maxLightIntensity = 3.0f;
	public float minLightIntensity = 1.0f;

	private Dictionary<GameObject, float> projectiles;
	private Rigidbody2D rb;

	
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		projectiles = new Dictionary<GameObject, float> ();
	}
	
	// Update is called once per frame
	void Update () {
		// Projectile code
		if (Input.GetButtonDown("Fire1") && projectileLimit > projectiles.Count) {
			// Spawns a projectile starting at the player,
			// with an offset on the z-axis so that they can be seen
			projectiles.Add(Instantiate(
				projectile, 
				new Vector2(transform.position.x, transform.position.y), 
				Quaternion.identity
				) as GameObject,
			                Time.time);
		}

		// Update the projectiles
		UpdateProjectiles ();

		// Jump put in update to make it more responsive
		if (IsGrounded () && (Input.GetButtonDown ("Jump"))) {
			rb.velocity = new Vector2(
				rb.velocity.x, 
				jumpSpeed
				);
		}

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
		foreach (KeyValuePair<GameObject, float> k in projectiles) {
			if (k.Value > Time.time + k.Key.GetComponent<ProjectileController>().lifeTime) {
				Destroy(k.Key.gameObject);
				projectiles.Remove(k.Key);
			} else if (k.Key == null) {
				// In case the object has already been destroyed
				projectiles.Remove(k.Key);
			} else {

			}
		}
	}
}
