using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFollow : MonoBehaviour {

	// For raycasting
	Plane hitplane;

	// Keeps initial z-axis position
	float zPos;

	// Wind vfx object
	public GameObject windEffect;

	// For smoothly turning off the wind effect
	private bool blowing = true;
	private bool stoppingBlowing = false;
	private float timer = 0;
	private float lifetime;


	// Cursor icon in a canvas somewhere
	public GameObject playerSprite;
	public Camera mainCamera;

	public bool displayMouse = false;

	void Start()
	{
		StopBlowEffect ();
		Cursor.visible = displayMouse;
		hitplane = new Plane( Vector3.forward, transform.position);
		zPos = transform.position.z;

		lifetime = windEffect.GetComponent<ParticleSystem> ().main.startLifetime.constant;
	}

	void Update() {
        // ------------------------------- MOBILE CHANGE -------------------------------
        // Update position of the cursor game object
        //if (Input.touchCount > 0)
        //{
        //    // 10 is camera distance
        //    // Get position of touch on screen
        //    Vector3 point = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 10);
        //    Vector3 pos = Camera.main.ScreenToWorldPoint(point);
        //    transform.position = new Vector3(pos.x, pos.y, pos.z);
        //}

        // cast the ray
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 

		// update position of the cursor game objects (world space)
		float distance;
		if (hitplane.Raycast(ray, out distance)){
			transform.position = ray.GetPoint(distance); 
		}
		transform.position = new Vector3 (transform.position.x, transform.position.y, zPos);

		// Update cursor icon's position in the UI (screen space)
		playerSprite.transform.position = mainCamera.WorldToScreenPoint(transform.position);

		if (stoppingBlowing) 
		{
			if (timer >= lifetime) {
				stoppingBlowing = false;
			}
			else {
				timer += Time.deltaTime;
			}
		}
	}

	public void BlowEffect(Vector3 direction)
	{
		// Start wind particles if they're not already going
		if (!blowing)
		{
			windEffect.GetComponent<ParticleSystem> ().Play ();
			blowing = true;
		}

		// Convert direction to angle
		float angle = Mathf.Atan2(direction.y, direction.x);
		windEffect.transform.rotation = Quaternion.AngleAxis(angle * 180 / Mathf.PI, Vector3.forward);
	}

	public void StopBlowEffect()
	{
		// Stop wind particles if they're still going
		if (blowing) 
		{
			timer = 0;
			stoppingBlowing = true;
			blowing = false;
			windEffect.GetComponent<ParticleSystem> ().Stop (true, ParticleSystemStopBehavior.StopEmitting);
		}
	}
}