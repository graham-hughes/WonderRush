using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {
	// Destroys the procedurally generated asteroids when shot with lasers

	public ParticleSystem exploding;
	private bool sendOffScreen;
	public GameObject asteroid;
	public Rigidbody asteroidRigid;

	private void OnParticleCollision ()
	{
		asteroidRigid.AddForce (Random.Range (0, 100), Random.Range (0, 100), Random.Range (0, 100));
		exploding.Emit(10);
		StartCoroutine (destroyAsteroid ());
	}
	IEnumerator destroyAsteroid(){
		yield return new WaitForSeconds (0.8f);
		Destroy (asteroid);
	}
}
