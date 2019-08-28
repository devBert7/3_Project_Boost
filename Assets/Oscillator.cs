using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent] // Attribute
public class Oscillator : MonoBehaviour {
	[SerializeField] Vector3 movementVector = new Vector3(20f, 0f, 0f);
	[SerializeField] float period = 5f;

	float movementFactor; // Temp - 0 = not moved, 1 = 100% moved
	
	Vector3 startingPos;

	// Start is called before the first frame update
	void Start() {
		startingPos = transform.position;
	}

	// Update is called once per frame
	void Update() {
		if (period <= Mathf.Epsilon) {
			return;
		}
		float cycles = Time.time / period; // Grows continually from 0... if game time is 1 second, it's at 50% or half of a cycle. Time of 10 = 5 cycles.
		const float tau = Mathf.PI * 2f; // tau is 2 PI. 6.28...
		float rawSinWave = Mathf.Sin(cycles *  tau); // Goes from -1 to +1

		movementFactor = rawSinWave / 2f + .5f; // Divides by 2, from -.5 to +.5... adjusted by adding .5 -> Goes from 0 to 1;
		Vector3 offset = movementVector * movementFactor;  // 20 movement on x * (0 to 1) = percentage of movement
		transform.position = startingPos + offset;  // Actually makes the appropriate move
	}
}