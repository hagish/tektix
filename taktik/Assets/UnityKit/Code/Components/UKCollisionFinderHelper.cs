using UnityEngine;
using System.Collections;

/// <summary>
/// Contains all Unity collision callbacks. 
/// Attach to an GameObject to debug collisions.
/// </summary>
public class UKCollisionFinderHelper : MonoBehaviour {
	void OnCollisionEnter(Collision collision) {
		Debug.Log("OnCollisionEnter " + collision, gameObject);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		Debug.Log("OnCollisionEnter2D " + collision, gameObject);
	}
	
	void OnCollisionExit(Collision collision) {
		Debug.Log("OnCollisionExit " + collision, gameObject);
	}

	void OnCollisionExit2D(Collision2D collision) {
		Debug.Log("OnCollisionExit2D " + collision, gameObject);
	}
	
	void OnTriggerEnter(Collider other) {
		Debug.Log("OnTriggerEnter " + other, gameObject);
	}

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log("OnTriggerEnter2D " + other, gameObject);
	}

	void OnTriggerExit(Collider other) {
		Debug.Log("OnTriggerExit " + other, gameObject);
	}

	void OnTriggerExit2D(Collider2D other) {
		Debug.Log("OnTriggerExit2D " + other, gameObject);
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		Debug.Log ("OnControllerColliderHit " + hit, gameObject);
	}
}
