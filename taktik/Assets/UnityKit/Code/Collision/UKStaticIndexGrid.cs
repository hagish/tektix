using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// XZ grid
public class UKStaticIndexGrid : IUKStaticIndex
{
	float CELL_SIZE;
	int GRID_SIZE_IN_CELLS;

	float CELL_SIZE_HALF;
	int GRID_SIZE_IN_CELLS_HALF;

	byte[] grid;

	Vector3 root;

	public UKStaticIndexGrid(float cellSize, int gridSizeInCells) {
		CELL_SIZE = cellSize;
		GRID_SIZE_IN_CELLS = gridSizeInCells;

		CELL_SIZE_HALF = CELL_SIZE / 2f;
		GRID_SIZE_IN_CELLS_HALF = GRID_SIZE_IN_CELLS / 2;

		grid = new byte[GRID_SIZE_IN_CELLS * GRID_SIZE_IN_CELLS];

		this.root = AlignToGrid(new Vector3(-GRID_SIZE_IN_CELLS_HALF * CELL_SIZE, 0f, -GRID_SIZE_IN_CELLS_HALF * CELL_SIZE));
	}

	UKTuple<int, int> CalculateIndexXZ(Vector3 p) {
		p = p - root;
		int x = Mathf.FloorToInt(p.x / CELL_SIZE);
		//int y = Mathf.FloorToInt(p.y / CELL_SIZE);
		int z = Mathf.FloorToInt(p.z / CELL_SIZE);
		return new UKTuple<int,int>(x,z);
	}

	// x,z in local cell space
	Vector3 CellCenter(int x, int z) {
		float dx = (float)x * CELL_SIZE + CELL_SIZE_HALF;
		float dz = (float)z * CELL_SIZE + CELL_SIZE_HALF;
		return root + new Vector3(dx, 0f, dz);
	}

	int CalculateIndex(Vector3 p) {
		var t = CalculateIndexXZ(p);
		return CalculateIndex(t.a, t.b);
	}

	// x,z in local cell space
	int CalculateIndex(int x, int z) {
		if (x >= 0 && x < GRID_SIZE_IN_CELLS &&
		    z >= 0 && z < GRID_SIZE_IN_CELLS) {
			return x + z * GRID_SIZE_IN_CELLS;
		} else {
			return -1;
		}
	}

	private Vector3 AlignToGrid(Vector3 p) {
		int x = Mathf.FloorToInt(p.x / CELL_SIZE);
		int y = Mathf.FloorToInt(p.y / CELL_SIZE);
		int z = Mathf.FloorToInt(p.z / CELL_SIZE);

		return new Vector3(CELL_SIZE * (float)x, CELL_SIZE * (float)y, CELL_SIZE * (float)z);
	}

	private IEnumerable<int> RasterObject(IUKStaticObject o) {
		Bounds b = o.SpatialBounds();

		var min = AlignToGrid(b.min);
		var max = AlignToGrid(b.max);

		var y = AlignToGrid(b.center).y;

		for(float x = min.x; x <= max.x; x += CELL_SIZE) {
			for(float z = min.z; z <= max.z; z += CELL_SIZE) {
				var p = new Vector3(x, y, z);
				if (o.SpatialIsSolidAt(p + Vector3.one * CELL_SIZE_HALF)) {
					yield return CalculateIndex(p);
				}
			}
		}
	}

	public void AddObject(IUKStaticObject o) {
		foreach(var index in RasterObject(o)) {
			if (index >= 0) grid[index] += 1;
		}
	}

	public void RemoveObject(IUKStaticObject o) {
		foreach(var index in RasterObject(o)) {
			if (index >= 0) grid[index] -= 1;
		}
	}

	private Vector3 CalculateResolveVector(Vector3 cellCenter, Vector3 p, float radius) {
		var d_cellCenter_p = (p - cellCenter).normalized;
		var pa_ = cellCenter + d_cellCenter_p * CELL_SIZE_HALF;
		var pb_ = p + (-1f * d_cellCenter_p) * radius;
		var overlap = Vector3.Distance(pa_, pb_);
		return d_cellCenter_p * (radius - overlap);
	}

	public bool Query(Vector3 pos, float radius, Vector3 movement, out Vector3 resolveAfterMovement) {
		resolveAfterMovement = Vector3.zero;

		var c = CalculateIndexXZ(pos + movement);
		int x0 = c.a;
		int y0 = c.b;
		int r = 1 + Mathf.FloorToInt(radius / CELL_SIZE);

		int x = r, y = 0;
		int radiusError = 1-x;
		int i = 0;
		
		while(x >= y) {
			for(i = -y + x0; i <= y + x0; ++i) {
				int a = i;
				int b = x + y0;
				int index = CalculateIndex(a,b);
				if (index < 0 || grid[index] > 0) {
					// collision, so push away
					resolveAfterMovement = CalculateResolveVector(CellCenter(a,b), pos + movement, radius);
					return true;
				}
			}
			
			for(i = -x + x0; i <= x + x0; ++i) {
				int a = i;
				int b = y + y0;
				int index = CalculateIndex(a,b);
				if (index < 0 || grid[index] > 0) {
					// collision, so push away
					resolveAfterMovement = CalculateResolveVector(CellCenter(a,b), pos + movement, radius);
					return true;
				}
			}

			for(i = -x + x0; i <= x + x0; ++i) {
				int a = i;
				int b = -y + y0;
				int index = CalculateIndex(a,b);
				if (index < 0 || grid[index] > 0) {
					// collision, so push away
					resolveAfterMovement = CalculateResolveVector(CellCenter(a,b), pos + movement, radius);
					return true;
				}
			}

			for(i = -y + x0; i <= y + x0; ++i) {
				int a = i;
				int b = -x + y0;
				int index = CalculateIndex(a,b);
				if (index < 0 || grid[index] > 0) {
					// collision, so push away
					resolveAfterMovement = CalculateResolveVector(CellCenter(a,b), pos + movement, radius);
					return true;
				}
			}
			
			y++;
			
			if (radiusError<0) {
				radiusError += 2 * y + 1;
			} else {
				x--;
				radiusError+= 2 * (y - x + 1);
			}
		}

		return false;
	}
	
	public void OnDrawGizmos() {
		for(int x = 0; x < GRID_SIZE_IN_CELLS; ++x) {
			for(int z = 0; z < GRID_SIZE_IN_CELLS; ++z) {
				var index = CalculateIndex(x,z);
				if (index >= 0) {
					Gizmos.color = grid[index] > 0 ? Color.red : Color.green;
					var c = root + new Vector3(x * CELL_SIZE, 0f, z * CELL_SIZE) + Vector3.one * CELL_SIZE_HALF;
					Gizmos.DrawWireCube(c, Vector3.one * CELL_SIZE);
				}
			}
		}
	}

}