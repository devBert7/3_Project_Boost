using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
	// Expose to inspector without allowing change in other scripts.. (use [SerializeField] rather than public)
	[SerializeField] float rcsThrust = 250f;
	[SerializeField] float mainThrust = 20f;
	[SerializeField] AudioClip mainEngine;
	[SerializeField] AudioClip victory;
	[SerializeField] AudioClip explosion;
	[SerializeField] ParticleSystem engineParticles;
	[SerializeField] ParticleSystem victoryParticles;
	[SerializeField] ParticleSystem explosionParticles;
	[SerializeField] float levelLoadDelay = 2f;

	// Access inspector components
	Rigidbody rigidbody;
	AudioSource audioSource;

	// States
	enum State {Alive, Dying, Transcending};
	State state = State.Alive;
	bool disableCollisions = false;

	// Start is called before the first frame update
	void Start() {
		rigidbody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()	{
		if (state == State.Alive) {
			ThrustResponse();
			RotationResponse();
		}
		if (Debug.isDebugBuild) {
			DebugResponse();
		}
	}

	void DebugResponse() {
		if (Input.GetKeyDown(KeyCode.L)) {
			Transcending();
		} else if (Input.GetKeyDown(KeyCode.C)) {
			disableCollisions = !disableCollisions;
		}
	}

	void OnCollisionEnter (Collision collision) {
		if (state != State.Alive || disableCollisions) {
			return;
		}

		switch (collision.gameObject.tag) {
			case "Friendly":
				break;
			case "Finish":
				VictorySequence();
				break;
			default:
				DeathSequence();
				break;
		}
	}

	void VictorySequence() {
		state = State.Transcending;
		audioSource.Stop();
		audioSource.PlayOneShot(victory);
		victoryParticles.Play();
		Invoke("Transcending", levelLoadDelay);
	}

	void DeathSequence() {
		state = State.Dying;
		audioSource.Stop();
		audioSource.PlayOneShot(explosion);
		explosionParticles.Play();
		Invoke("Dying", levelLoadDelay);
	}

	void ThrustResponse() {
		if (Input.GetKey(KeyCode.Space)) {
			ApplyThrust();
		} else {
			audioSource.Stop();
			engineParticles.Stop();
		}
	}

	void ApplyThrust() {
		rigidbody.AddRelativeForce(Vector3.up * mainThrust);
		if (!audioSource.isPlaying) {
			audioSource.PlayOneShot(mainEngine);
		}
		engineParticles.Play();
	}

	void RotationResponse() {
		rigidbody.freezeRotation = true; // Take control of rotation manually
		float rotationThisFrame = rcsThrust * Time.deltaTime;

		if (Input.GetKey(KeyCode.A)) {
			transform.Rotate(Vector3.forward * rotationThisFrame);
		} else if (Input.GetKey(KeyCode.D)) {
			transform.Rotate(-Vector3.forward * rotationThisFrame);
		}

		rigidbody.freezeRotation = false; // Resume physics control of rotation
	}

	void Transcending() {
		int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
		int nextSceneIndex = activeSceneIndex + 1;
		if (nextSceneIndex < SceneManager.sceneCountInBuildSettings) {
			SceneManager.LoadScene(nextSceneIndex);
		} else {
			SceneManager.LoadScene(0);
		}
	}

	void Dying() {
		SceneManager.LoadScene(0);
	}
}
