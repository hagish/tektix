using UnityEngine;
using System.Collections;

public static class UKEquation {

	// simple throw parabel with points (0,0) (w,0) (w/2,h)
	public static float ThrowParable(float height, float width, float x) {
		return -4f * height / width / width * x * x + 4f * height / width * x;
	}
}