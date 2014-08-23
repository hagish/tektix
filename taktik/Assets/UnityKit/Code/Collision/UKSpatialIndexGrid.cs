using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// XZ grid
public class UKSpatialIndexGrid : IUKSpatialIndex
{
	float CELL_SIZE;
	int GRID_SIZE_IN_CELLS;
	int GRID_SIZE_IN_CELLS_HALF;

	Dictionary<IUKSpatialObject,int> overlap;

	Dictionary<IUKSpatialObject,int>[] grid;

	public UKSpatialIndexGrid(float cellSize, int gridSizeInCells) {
		CELL_SIZE = cellSize;
		GRID_SIZE_IN_CELLS = gridSizeInCells;
		GRID_SIZE_IN_CELLS_HALF = GRID_SIZE_IN_CELLS / 2;

		overlap = new Dictionary<IUKSpatialObject,int>();
		grid = new Dictionary<IUKSpatialObject,int>[GRID_SIZE_IN_CELLS * GRID_SIZE_IN_CELLS];
	}

	int CalculateIndex(Vector3 p) {
		int x = Mathf.FloorToInt(p.x / CELL_SIZE);
		//int y = Mathf.FloorToInt(p.y / CELL_SIZE);
		int z = Mathf.FloorToInt(p.z / CELL_SIZE);
		return CalculateIndex(x,z);
	}

	int CalculateIndex(int x, int z) {
		x = x + GRID_SIZE_IN_CELLS_HALF;
		z = z + GRID_SIZE_IN_CELLS_HALF;

		if (x >= 0 && x < GRID_SIZE_IN_CELLS &&
		    z >= 0 && z < GRID_SIZE_IN_CELLS) {
			return x + z * GRID_SIZE_IN_CELLS;
		} else {
			return -1;
		}
	}

	void EnsureIndex(int index) {
		if (grid[index] == null) grid[index] = new Dictionary<IUKSpatialObject, int>();
	}

	public void AddObject(IUKSpatialObject o) {
		Profiler.BeginSample("AddObject");

		int index = CalculateIndex(o.SpatialPosition());
		if (index >= 0) {
			EnsureIndex(index);
			var l = grid[index];
			if (!l.ContainsKey(o)) l.Add(o, index);
		} else {
			if (!overlap.ContainsKey(o)) overlap.Add(o, -1);
		}

		Profiler.EndSample();
	}
	
	public void RemoveObject(IUKSpatialObject o) {
		Profiler.BeginSample("RemoveObject");
		
		int index = CalculateIndex(o.SpatialPosition());
		if (index >= 0) {
			EnsureIndex(index);
			var l = grid[index];
			l.Remove(o);
		} else {
			overlap.Remove(o);
		}
		
		Profiler.EndSample();
	}
	
	public void NotifyObjectMove(IUKSpatialObject o, Vector3 oldPosition) {
		Profiler.BeginSample("NotifyObjectMove");

		int oldIndex = CalculateIndex(oldPosition);
		int newIndex = CalculateIndex(o.SpatialPosition());
		if (oldIndex == newIndex) {
			Profiler.EndSample();
			return;
		}


		if (oldIndex >= 0) {
			EnsureIndex(oldIndex);
			var l = grid[oldIndex];
			l.Remove(o);
		} else {
			overlap.Remove(o);
		}
		
		AddObject(o);
		
		Profiler.EndSample();
	}
	
	public void Query(Vector3 pos, float radius, UKList<IUKSpatialObject> result) {
		int cx = Mathf.FloorToInt(pos.x / CELL_SIZE);
		//int cy = Mathf.FloorToInt(pos.y / CELL_SIZE);
		int cz = Mathf.FloorToInt(pos.z / CELL_SIZE);

		int d = Mathf.FloorToInt(radius / CELL_SIZE);

		bool addOverlap = false;

		// XZ
		for (int x = cx-d; x <= cx+d; ++x) {
			for (int z = cz-d; z <= cz+d; ++z) {
				var index = CalculateIndex(x,z);
				if (index >= 0) {
					EnsureIndex(index);
					var l = grid[index];
					foreach(var o in l.Keys) {
						result.Add(o);
					}
				} else {
					addOverlap = true;
				}
			}
		}

		if (addOverlap) {
			foreach(var o in overlap.Keys) {
				var p = o.SpatialPosition();
				var minAxisDist = Mathf.Min(Mathf.Abs(p.x - pos.x), Mathf.Abs(p.z - pos.z));
				if (minAxisDist <= o.SpatialRadius() + radius) {
					result.Add(o);
				}
			}
		}
	}
	
	public void OnDrawGizmos() {
		Gizmos.color = Color.red;
		foreach(var o in overlap.Keys) {
			Gizmos.DrawSphere(o.SpatialPosition(), o.SpatialRadius() * 1.1f);
		}

		var rootCenter = new Vector3(-1f * (float)GRID_SIZE_IN_CELLS_HALF * CELL_SIZE + CELL_SIZE / 2f, 0f, -1f * (float)GRID_SIZE_IN_CELLS_HALF * CELL_SIZE + CELL_SIZE / 2f);
		var size = Vector3.one * CELL_SIZE;
		for (int x = 0; x < GRID_SIZE_IN_CELLS; ++x) {
			for (int z = 0; z < GRID_SIZE_IN_CELLS; ++z) {
				var p = rootCenter + new Vector3((float)x * CELL_SIZE, 0f, (float)z * CELL_SIZE);
				Gizmos.DrawWireCube(p, size); 
			}
		}
	}

}