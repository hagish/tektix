using UnityEngine;
using System.Collections;

public static class UKRaycastUtils {
	// steps >= 3
	// returns point, distance, normal
	public static UKTuple<Vector3, float, Vector3> averageRaycast(Vector3 start, Vector3 end, Vector3 up, int steps, int layerMask, float raycastDist, bool debug)
	{
		int hits = 0;
		
		Vector3 avgPoint = Vector3.zero;
		float avgDistance = 0f;
		Vector3 avgNormal = Vector3.zero;
		
		for (int i = 0; i < steps; ++i)
		{
			float f = (float)i / (float)(steps - 1);
			RaycastHit hit;
			
			Vector3 pos = Vector3.Lerp(start, end, f);
			if (Physics.Raycast(pos, -up, out hit, raycastDist, layerMask))
			{
				++hits;
				avgPoint = avgPoint + hit.point;
				avgDistance = avgDistance + hit.distance;
				avgNormal = avgNormal + hit.normal;
				
				if (debug)
				{
					Gizmos.color = Color.yellow;
					Gizmos.DrawRay(hit.point, hit.normal * hit.distance);
				}
			}
		}
		
		if (hits > 0)
		{
			avgPoint = avgPoint / (float)hits;
			avgDistance = avgDistance / (float)hits;
			avgNormal = avgNormal / (float)hits;
			
			if (debug)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawRay(avgPoint, avgNormal * avgDistance);
			}
		}
		
		return new UKTuple<Vector3, float, Vector3>(avgPoint, avgDistance, avgNormal);
	}	
}
