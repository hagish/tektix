using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class UKTransformExtension {
	public static void ResetTransformLocal(this Transform transform)
	{
		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
	}
}
