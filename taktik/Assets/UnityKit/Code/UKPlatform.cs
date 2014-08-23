using UnityEngine;
using System.Collections;

public static class UKPlatform {
	public static float ScreenDistanceToInch(float screenDist) {
		float dpi = Screen.dpi;
		if (dpi <= 0f) dpi = 72f;
		return screenDist / dpi;
	}
}
