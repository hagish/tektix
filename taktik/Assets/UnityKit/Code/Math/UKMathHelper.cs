using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class UKMathHelper {

	public static float MinDistanceToLine(Vector3 vec, Vector3 linePoint, Vector3 lineDir ) {
		return Vector3.Distance (ProjectOntoLine (vec, linePoint, lineDir), vec);
	}

    // http://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment
    public static float MinDistanceToLineSegment(Vector2 start, Vector2 end, Vector2 point)
    { 
        // sqdist
        var l2 = Vector2.Distance(start, end);
        l2 *= l2;

        if (l2 == 0f) return Vector2.Distance(point, start);
        var t = ((point.x - start.x) * (end.x - start.x) + (point.y - start.y) * (end.y - start.y)) / l2;
        if (t < 0) return Vector2.Distance(point, start);
        if (t > 1) return Vector2.Distance(point, end);
        return Vector2.Distance(point, new Vector2(start.x + t * (end.x - start.x), start.y + t * (end.y - start.y)));
    }

	public static float MinDistanceToAABB(Vector3 vec, Vector3 pMin, Vector3 pMax) {
		var b = new Bounds();
		b.SetMinMax(pMin, pMax);

		// inside?
		if (b.Contains(vec)) return 0f;
		else return Mathf.Sqrt(b.SqrDistance(vec));
	}

	public static Vector3 ProjectOntoLine( Vector3 vec, Vector3 linePoint, Vector3 lineDir )
	{
		var localP = vec - linePoint;
		return linePoint + Vector3.Project (localP, lineDir);
	}

	public static Vector3 ProjectOntoPlane( Vector3 vec, Vector3 planePoint, Vector3 planeNormal ) {
		var d = vec - planePoint;
		return planePoint + ProjectOntoPlane(d, planeNormal);
	}

	public static Vector3 ProjectOntoPlane( Vector3 vec, Vector3 planeNormal )
	{
	    return vec - Vector3.Dot(vec, planeNormal) * planeNormal;
	}
	
	public static bool IsSameDirection(Vector3 dir1, Vector3 dir2){
		return Vector3.Dot(dir1, dir2) > 0.0f;	
	}
	
	/*
	 * circles hit <=> distance < 0
	 */
	public static float CalculateCircleMinDistance(Vector3 posA, float radiusA, Vector3 posB, float radiusB){
		Vector3 d = posA - posB;
		return d.magnitude - radiusA - radiusB;
	}
	
	public static Vector3 GetOrthogonalUnitVector(Vector3 normal)
	{
		Vector3 other = Vector3.zero;
		
		if (normal.x == 0.0f) {
			other.x = 1.0f;	
		} else {
			other.x = -normal.y;
			other.y = normal.x;
		}
		
		Vector3 ortho = ProjectOntoPlane(other.normalized, normal);
		
		return ortho;
	}
	
	public static Vector3 RotateVectorAroundAxis(Vector3 v, Vector3 axis, float rotationDegree)
	{
		Quaternion randomRotation = Quaternion.AngleAxis(rotationDegree, axis);
		return randomRotation * v;
	}
	
	public static Vector3 GetRandomOrthogonalUnitVector(Vector3 normal)
	{
		Vector3 ortho = GetOrthogonalUnitVector(normal);
		Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), normal);
		return randomRotation * ortho;
	}
	
	public static float MapIntoRange(float srcValue, float srcMin, float srcMax, float dstMin, float dstMax)
	{
		float r =  (Mathf.Clamp(srcValue, srcMin, srcMax) - srcMin) / (srcMax - srcMin);
		return dstMin + (dstMax - dstMin) * r;
	}
	
	public static float Round(float v, int decimalDigits)
	{
		float f = Mathf.Pow(10, decimalDigits);
		v = Mathf.Round(v * f) / f;
		return v;
	}

	public static Vector3 ReflectVector(Vector3 incidentVector, Vector3 reflectionNormal) {
		return incidentVector - reflectionNormal * 2 * Vector3.Dot(incidentVector, reflectionNormal);
	}

	// returns 0f -> 1f, area in between is of length 2f*lengthOnOneSide, center line returns 0.5f
	public static float Gradient(Vector3 linePoint, Vector3 lineDir, Vector3 pointsToZeroSide, float lengthOnOneSide, Vector3 p) {
		var pOnPlane = UKMathHelper.ProjectOntoPlane(p, linePoint, Vector3.Cross(lineDir, pointsToZeroSide).normalized);
		float d = UKMathHelper.MinDistanceToLine(pOnPlane, linePoint, lineDir);
		var centerToPoint = pOnPlane - linePoint;
		if (Vector3.Dot(centerToPoint, pointsToZeroSide) > 0) {
			return UKMathHelper.MapIntoRange(d, 0f, lengthOnOneSide, 0.5f, 0f);
		} else {
			return UKMathHelper.MapIntoRange(d, 0f, lengthOnOneSide, 0.5f, 1f);
		}
	}


    public static IEnumerable<float> EnumValuesBetween(float min, float max, float delta)
    {
        if (delta <= 0f)
        {
            throw new System.ArgumentException("delta <= 0");
        }
        
        if (min >= max)
        {
            yield return max;
        }
        else
        {
            float p = min;
            while (p < max)
            {
                yield return p;
                p += delta;
            }
            yield return max;
        }
    }

}
