using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class UKRectExtension {
	public static bool ContainsOther(this Rect t, Rect other)
	{
		return t.xMin <= other.xMin && other.xMax <= t.xMax &&
			t.yMin <= other.yMin && other.yMax <= t.yMax;
	}
}