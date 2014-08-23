using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// XYZ hash
public class UKSpatialIndexHash : IUKSpatialIndex
{
	const float CELL_SIZE = 1f;

	Dictionary<uint, Dictionary<IUKSpatialObject, uint>> spatialHashTable = new Dictionary<uint, Dictionary<IUKSpatialObject, uint>>();

	void EnsureHash(uint hash) {
		if (!spatialHashTable.ContainsKey(hash)) {
			spatialHashTable[hash] = new Dictionary<IUKSpatialObject, uint>();
		}
	}

	uint CalculateHash(int x, int y, int z) {
		// FNV hash
		uint h = 2166136261;
		h = (h * 16777619) ^ (uint)x;
		h = (h * 16777619) ^ (uint)y;
		h = (h * 16777619) ^ (uint)z;
		//Debug.Log(string.Format("{0} {1} {2} -> {3}", x,y,z, h));
		return h;
	}

	IEnumerable<UKTuple<int, int, int>> EnumCellXYZOfBounds(Bounds b) {
		int minx = Mathf.FloorToInt(b.min.x / CELL_SIZE);
		int miny = Mathf.FloorToInt(b.min.y / CELL_SIZE);
		int minz = Mathf.FloorToInt(b.min.z / CELL_SIZE);
		
		int maxx = Mathf.FloorToInt(b.max.x / CELL_SIZE);
		int maxy = Mathf.FloorToInt(b.max.y / CELL_SIZE);
		int maxz = Mathf.FloorToInt(b.max.z / CELL_SIZE);
		
		for (int z = minz; z <= maxz; ++z) {
			for (int y = miny; y <= maxy; ++y) {
				for (int x = minx; x <= maxx; ++x) {
					yield return new UKTuple<int,int,int>(x,y,z);	
				}
			}
		}
	}

	Bounds ExtendBountToCells(Bounds b) {
		int minx = Mathf.FloorToInt(b.min.x / CELL_SIZE);
		int miny = Mathf.FloorToInt(b.min.y / CELL_SIZE);
		int minz = Mathf.FloorToInt(b.min.z / CELL_SIZE);
		
		int maxx = Mathf.FloorToInt(b.max.x / CELL_SIZE);
		int maxy = Mathf.FloorToInt(b.max.y / CELL_SIZE);
		int maxz = Mathf.FloorToInt(b.max.z / CELL_SIZE);

		var min = new Vector3((float)minx * CELL_SIZE, (float)miny * CELL_SIZE, (float)minz * CELL_SIZE);
		var max = new Vector3((float)(maxx + 1) * CELL_SIZE, (float)(maxy + 1) * CELL_SIZE, (float)(maxz + 1) * CELL_SIZE);
		return new Bounds((max + min) / 2f, max - min);
	}

	IEnumerable<uint> EnumHashsOfBounds(Bounds b) {
		foreach(var t in EnumCellXYZOfBounds(b)) {
			yield return CalculateHash(t.a,t.b,t.c);	
		}
	}

	public void AddObject(IUKSpatialObject o) {
		Profiler.BeginSample("AddObject");

		foreach(var h in EnumHashsOfBounds(o.SpatialBounds())) {
			EnsureHash(h);
			var l = spatialHashTable[h];
			if (!l.ContainsKey(o)) l.Add(o, h);
		}

		Profiler.EndSample();
	}
	
	public void RemoveObject(IUKSpatialObject o) {
		Profiler.BeginSample("RemoveObject");

		foreach(var h in EnumHashsOfBounds(o.SpatialBounds())) {
			EnsureHash(h);
			var l = spatialHashTable[h];
			l.Remove(o);
		}

		Profiler.EndSample();
	}

	public void NotifyObjectMove(IUKSpatialObject o, Vector3 oldPosition) {
		Profiler.BeginSample("NotifyObjectMove");

		var size = Vector3.one * o.SpatialRadius() * 2f;
		var b0 = ExtendBountToCells(new Bounds(oldPosition, size));
		var b1 = ExtendBountToCells(new Bounds(o.SpatialPosition(), size));

		var dc = Vector3.Distance(b0.center, b1.center);
		var ds = Vector3.Distance(b0.size, b1.size);

		if (dc < Mathf.Epsilon && ds < Mathf.Epsilon) {
			Profiler.EndSample();
			return;
		}

		foreach(var h in EnumHashsOfBounds(new Bounds(oldPosition, Vector3.one * o.SpatialRadius() * 2f))) {
			EnsureHash(h);
			var l = spatialHashTable[h];
			l.Remove(o);
		}

		AddObject(o);

		Profiler.EndSample();
	}
	
	public void Query(Vector3 pos, float radius, UKList<IUKSpatialObject> result) {
		var b = new Bounds(pos, Vector3.one * radius * 2f);

		int minx = Mathf.FloorToInt(b.min.x / CELL_SIZE);
		int miny = Mathf.FloorToInt(b.min.y / CELL_SIZE);
		int minz = Mathf.FloorToInt(b.min.z / CELL_SIZE);
		
		int maxx = Mathf.FloorToInt(b.max.x / CELL_SIZE);
		int maxy = Mathf.FloorToInt(b.max.y / CELL_SIZE);
		int maxz = Mathf.FloorToInt(b.max.z / CELL_SIZE);
		
		for (int z = minz; z <= maxz; ++z) {
			for (int y = miny; y <= maxy; ++y) {
				for (int x = minx; x <= maxx; ++x) {
					var h = CalculateHash(x,y,z);
					if (spatialHashTable.ContainsKey(h)) {
						var l = spatialHashTable[h];
						foreach(var p in l) {
							result.Add(p.Key);
						}
					}
				}
			}
		}
	}

	public void OnDrawGizmos() {
		foreach(var p in spatialHashTable) {
			Gizmos.color = UKDebug.GetColor((int)p.Key);
			foreach(var o in p.Value.Keys) {
				Gizmos.DrawSphere(o.SpatialPosition(), o.SpatialRadius() * 1.1f);

				foreach(var t in EnumCellXYZOfBounds(o.SpatialBounds())) {
					var pmin = new Vector3((float)t.a * CELL_SIZE, (float)t.b * CELL_SIZE, (float)t.c * CELL_SIZE);
					Gizmos.color = UKDebug.GetColor((int)CalculateHash(t.a, t.b, t.c));
					Gizmos.DrawWireCube(pmin + Vector3.one * CELL_SIZE * 0.5f, Vector3.one * CELL_SIZE);
				}
			}
		}
	}
}