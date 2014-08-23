using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IUKSpatialObject {
	Vector3 SpatialPosition();
	float SpatialRadius();
	int SpatialOrder();
}

public static class IUKSpatialObjectExtension {
	public static Bounds SpatialBounds(this IUKSpatialObject self) {
		return new Bounds(self.SpatialPosition(), Vector3.one * self.SpatialRadius() * 2f);
	}
}

public interface IUKSpatialIndex
{
	void AddObject(IUKSpatialObject o);
	void RemoveObject(IUKSpatialObject o);
	void NotifyObjectMove(IUKSpatialObject o, Vector3 oldPosition);

	// can contains more that necessary but must contain atleast all matching
	// this query does not clear result
	void Query(Vector3 pos, float radius, UKList<IUKSpatialObject> result);

	void OnDrawGizmos();
}
