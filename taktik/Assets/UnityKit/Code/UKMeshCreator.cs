using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class UKMeshCreator {
	public static Mesh Cylinder(int steps, float radius, float height) {
		Mesh mesh = new Mesh();

		List<Vector3> newVertices = new List<Vector3>();
		List<Vector2> newUV = new List<Vector2>();
		List<int> newTriangles = new List<int>();

		Vector3 bottom = new Vector3(0f, 0f, 0f);
		Vector3 top = new Vector3(0f, height, 0f);

		// bottom
		newVertices.Add(bottom);
		for(int i = 0; i < steps; ++i) {
			float angle = (float)i/(float)steps * 360f;
			var p = Quaternion.Euler(Vector3.up * angle) * Vector3.forward * radius;
			newVertices.Add(p);
		}

		// top
		newVertices.Add(top);
		for(int i = 0; i < steps; ++i) {
			float angle = (float)i/(float)steps * 360f;
			var p = Quaternion.Euler(Vector3.up * angle) * Vector3.forward * radius;
			newVertices.Add(top + p);
		}

		// close bottom
		for(int i = 0; i < steps; ++i) {
			newTriangles.Add(0);
			newTriangles.Add(1 + ((i+1) % steps));
			newTriangles.Add(1 + ((i+0) % steps));
		}

		// close top
		for(int i = 0; i < steps; ++i) {
			newTriangles.Add(steps + 1);
			newTriangles.Add(steps + 1 + 1 + ((i+0) % steps));
			newTriangles.Add(steps + 1 + 1 + ((i+1) % steps));
		}

		// sides
		for(int i = 0; i < steps; ++i) {
			int b0 = 1 + ((i+0) % steps);
			int b1 = 1 + ((i+1) % steps);

			int t0 = steps + 1 + 1 + ((i+0) % steps);
			int t1 = steps + 1 + 1 + ((i+1) % steps);

			newTriangles.Add(b0);
			newTriangles.Add(b1);
			newTriangles.Add(t0);

			newTriangles.Add(b1);
			newTriangles.Add(t1);
			newTriangles.Add(t0);
		}

		// just crappy uv
		for(int i = 0; i < newVertices.Count; ++i) {
			newUV.Add(new Vector2(0f, 0f));
		}

		mesh.vertices = newVertices.ToArray();
		mesh.uv = newUV.ToArray();
		mesh.triangles = newTriangles.ToArray();

		return mesh;
	}
}
