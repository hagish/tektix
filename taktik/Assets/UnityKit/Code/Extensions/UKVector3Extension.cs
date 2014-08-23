using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class UKVector3Extension {
	
	public static Vector3 SetX(this Vector3 v, float f)
	{
		return new Vector3(f, v.y, v.z);
	}

	public static Vector3 SetY(this Vector3 v, float f)
	{
		return new Vector3(v.x, f, v.z);
	}

	public static Vector3 SetZ(this Vector3 v, float f)
	{
		return new Vector3(v.x, v.y, f);
	}
}
