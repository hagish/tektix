using UnityEngine;
using System.Collections;

public static class UKDebug {

	public static Color[] Colors = new Color[]{
		Color.red,
		Color.blue,
		Color.cyan,
		Color.green,
		Color.magenta,
		Color.yellow,
	};

	public static Color GetColor(int index) {
		return Colors[Mathf.Abs(index) % Colors.Length];
	}

	private static Vector3 DrawSphereHelper(Vector3 arrow, Vector3 normal, float angle) {
		return Quaternion.Euler(normal * angle) * arrow;
	}

	public static void DrawSphere(Vector3 position, float radius, Color color, float time) {
        if (!Application.isEditor) return;

        int steps = 32;

		for(int i = 0; i < steps; ++i) {
			var p0 = position + DrawSphereHelper(Vector3.forward * radius, Vector3.right, (float)i/(float)steps * 360f);
			var p1 = position + DrawSphereHelper(Vector3.forward * radius, Vector3.right, (float)(i+1)/(float)steps * 360f);
			Debug.DrawLine(p0,p1,color,time);
		}

		for(int i = 0; i < steps; ++i) {
			var p0 = position + DrawSphereHelper(Vector3.forward * radius, Vector3.up, (float)i/(float)steps * 360f);
			var p1 = position + DrawSphereHelper(Vector3.forward * radius, Vector3.up, (float)(i+1)/(float)steps * 360f);
			Debug.DrawLine(p0,p1,color,time);
		}

		for(int i = 0; i < steps; ++i) {
			var p0 = position + DrawSphereHelper(Vector3.right * radius, Vector3.forward, (float)i/(float)steps * 360f);
			var p1 = position + DrawSphereHelper(Vector3.right * radius, Vector3.forward, (float)(i+1)/(float)steps * 360f);
			Debug.DrawLine(p0,p1,color,time);
		}
	}
}