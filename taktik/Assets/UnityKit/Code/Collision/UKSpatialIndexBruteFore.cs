using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UKSpatialIndexBruteFore : IUKSpatialIndex
{
	UKList<IUKSpatialObject> Objects = new UKList<IUKSpatialObject>();

	public void AddObject(IUKSpatialObject o) {
		Objects.Add(o);
	}

	public void RemoveObject(IUKSpatialObject o) {
		Objects.Remove(o);
	}

	public void NotifyObjectMove(IUKSpatialObject o, Vector3 oldPosition) {

	}

	public void Query(Vector3 pos, float radius, UKList<IUKSpatialObject> result) {
		foreach(var o in Objects) {
			result.Add(o);
		}
	}

	public void OnDrawGizmos ()
	{
	}
}
