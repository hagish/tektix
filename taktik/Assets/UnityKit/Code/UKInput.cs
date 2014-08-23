using UnityEngine;
using System.Collections;

public static class UKInput {

	public static Vector3 GetMovementRaw() {
		var h = Input.GetAxisRaw ("Horizontal");
		var v = Input.GetAxisRaw ("Vertical");
		return new Vector3(h, 0f, v);
	}

	public static IEnumerator CoWaitForInput() {
		bool isWaiting = true;
		while (isWaiting) {
			yield return null;
			if (Input.GetMouseButtonDown(0)) isWaiting = false;
			if (Input.GetMouseButtonDown(1)) isWaiting = false;
			if (Input.touchCount > 0) isWaiting = false;
			if (Input.anyKey) isWaiting = false;
		}
	}

	public static IEnumerator CoWaitForNoInput() {
		while (Input.GetMouseButtonDown(0) || 
		       Input.GetMouseButtonDown(1) || 
		       Input.touchCount > 0 ||
		       Input.anyKey) {
			yield return null;
		}
	}
}
