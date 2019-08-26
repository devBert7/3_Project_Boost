using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
	// Expose to inspector without allowing change in other scripts.. (use [SerializeField] rather than public)
	[SerializeField] float rcsThrust = 250f;
	[SerializeField] float mainThrust = 25f;

	// Access inspector components
	Rigidbody rigidbody;
	AudioSource audioSource;

	// Start is called before the first frame update
	void Start() {
		rigidbody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()	{
		Thrust();
		Rotate();
	}

	void OnCollisionEnter (Collision collision) {
		switch (collision.gameObject.tag) {
			case "Friendly":
				print("You're ok");
				break;
			default:
				print("You died!");
				break;
		}
	}

	void Thrust() {
		if (Input.GetKey(KeyCode.Space)) {
			rigidbody.AddRelativeForce(Vector3.up * mainThrust);
			if (!audioSource.isPlaying) {
				audioSource.Play();
				}
		} else {
			audioSource.Stop();
		}
	}

	void Rotate() {
		rigidbody.freezeRotation = true; // Take control of rotation manually
		float rotationThisFrame = rcsThrust * Time.deltaTime;

		if (Input.GetKey(KeyCode.A)) {
			transform.Rotate(Vector3.forward * rotationThisFrame);
		} else if (Input.GetKey(KeyCode.D)) {
			transform.Rotate(-Vector3.forward * rotationThisFrame);
		}

		rigidbody.freezeRotation = false; // Resume physics control of rotation
	}
}
