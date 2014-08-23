using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class UKNumberSequence {
	
	public static IEnumerable<float> FromTo(float from, float to, int points) {
		if (points == 1) yield return from;
		else if (points == 2) {
			yield return from;
			yield return to;
		} else {
			float d = (to - from) / (float)(points - 1);
			for (int i = 0; i < points - 1; ++i) {
				yield return from + (float)i * d;
			}
			yield return to;
		}
	}
}
