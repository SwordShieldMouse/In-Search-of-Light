using UnityEngine;
using System.Collections;

public class PulseController : MonoBehaviour {
	private bool secondarySpawned;
	private float currentResource;

	// Use this for initialization
	void Start () {
		secondarySpawned = false;
	}
	
	// Update is called once per frame
	void Update () {
		currentResource = gameObject.GetComponent<PlayerController> ().currentResource;
		SecondaryCheck (SecondaryFireCheck());
	}

	IEnumerator FadeLight(Light light) {	
		yield return new WaitForSeconds (0.5f);
		for (float f = 1.0f; f >= 0; f -= 0.05f) {
			light.intensity = Mathf.Lerp(light.intensity, 0.0f, 0.4f);
			light.range = Mathf.Lerp(light.range, 0.0f, 0.4f);
			yield return new WaitForSeconds(0.05f);
		}
		Destroy (light);
		secondarySpawned = false;
	}

	void SecondaryCheck(Light light) {
		if (light != null) {
			StartCoroutine(FadeLight(light));
		}
	}

	void SetLight(Light light) {
		if (light != null) {
			// Effectiveness of light scales with resource bar
			light.intensity = 8.0f * currentResource;
			light.range = 70.0f * currentResource;
		
			light.transform.position.Set (
			light.transform.position.x,
			light.transform.position.y,
			-1.0f
			);
		}
	}

	Light SecondaryFireCheck() {
		// Secondary fire will utilise all of the resource bar
		if (Input.GetButtonDown ("Fire2") && currentResource > 0 && !secondarySpawned) {
			secondarySpawned = true;

			GameObject g = new GameObject("Pulse Holder");
			g.transform.parent = gameObject.transform;
			g.transform.position.Set(
				g.transform.position.x,
				g.transform.position.y,
				-1.0f
				);
			Light light = g.AddComponent<Light>();
			SetLight(light);
			
			gameObject.GetComponent<PlayerController> ().currentResource = 0.0f;
			return light;
		} else {
			return null;
		}
	}

}
