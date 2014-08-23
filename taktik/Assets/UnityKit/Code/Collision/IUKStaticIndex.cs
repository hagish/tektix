using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IUKStaticObject {
	Bounds SpatialBounds();
	bool SpatialIsSolidAt(Vector3 worldPos);
}

public interface IUKStaticIndex
{
	void AddObject(IUKStaticObject o);
	void RemoveObject(IUKStaticObject o);

	bool Query(Vector3 pos, float radius, Vector3 movement, out Vector3 resolveAfterMovement);

	void OnDrawGizmos();
}
