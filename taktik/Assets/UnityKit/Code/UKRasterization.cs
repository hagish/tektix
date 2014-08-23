using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class UKRasterization {
	private static void Swap<T>(ref T lhs, ref T rhs)
	{
		T temp;
		temp = lhs;
		lhs = rhs;
		rhs = temp;
	}


	// http://en.wikipedia.org/wiki/Midpoint_circle_algorithm
	// http://stackoverflow.com/questions/1201200/fast-algorithm-for-drawing-filled-circles
	public static IEnumerable<UKTuple<int,int>> CircleFilled(int x0, int y0, int radius) {
		int x = radius, y = 0;
		int radiusError = 1-x;
		int i = 0;

		while(x >= y) {
			//DrawPixel(-y + x0, x + y0);
			//DrawPixel(y + x0, x + y0);
			for(i = -y + x0; i <= y + x0; ++i) yield return new UKTuple<int, int>(i, x + y0);

			// DrawPixel(-x + x0, y + y0);
			// DrawPixel(x + x0, y + y0);
			for(i = -x + x0; i <= x + x0; ++i) yield return new UKTuple<int, int>(i, y + y0);

			// DrawPixel(-x + x0, -y + y0);
			// DrawPixel(x + x0, -y + y0);
			for(i = -x + x0; i <= x + x0; ++i) yield return new UKTuple<int, int>(i, -y + y0);

			// DrawPixel(-y + x0, -x + y0);
			// DrawPixel(y + x0, -x + y0);
			for(i = -y + x0; i <= y + x0; ++i) yield return new UKTuple<int, int>(i, -x + y0);

			y++;

			if (radiusError<0) {
				radiusError += 2 * y + 1;
			} else {
				x--;
				radiusError+= 2 * (y - x + 1);
			}
		}
	}


	// http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
	public static IEnumerable<UKTuple<int,int>> Line(int x0, int y0, int x1, int y1) {
		// dx := abs(x1-x0)
		int dx = Math.Abs(x1-x0);
		// dy := abs(y1-y0) 
		int dy = Math.Abs(y1-y0);
		// if x0 < x1 then sx := 1 else sx := -1
		int sx = 1;
		if (x0 >= x1) sx = -1; 
		// if y0 < y1 then sy := 1 else sy := -1
		int sy = 1;
		if (y0 >= y1) sy = -1;
		// err := dx-dy
		int err = dx-dy;

		// loop
		while(true) {
		// 	plot(x0,y0)
			yield return new UKTuple<int, int>(x0,y0);
		// 	if x0 = x1 and y0 = y1 exit loop
			if (x0 == x1 && y0 == y1) yield break;
		// 	e2 := 2*err
			int e2 = 2*err;
		// 	if e2 > -dy then
			if (e2 > -dy) {
		// 		err := err - dy
				err = err - dy;
		// 		x0 := x0 + sx
				x0 = x0 + sx;
		// 	end if
			}
		// 	if x0 = x1 and y0 = y1 then 
			if (x0 == x1 && y0 == y1) {
		// 	plot(x0,y0)
				yield return new UKTuple<int, int>(x0,y0);
		// 	exit loop
				yield break;
		// 	end if}
			}
		// 	if e2 <  dx then 
			if (e2 < dx) {
		// 		err := err + dx
				err = err + dx;
		// 		y0 := y0 + sy
				y0 = y0 + sy;
		// 	end if
			}
		// end loop
		}
	}

    /// <summary>
    /// Rasterize a collider by countX*countY equal distributed raycasts
    /// </summary>
    /// <param name="c"></param>
    /// <param name="countX"></param>
    /// <param name="countY"></param>
    /// <param name="border"></param>
    /// <returns></returns>
    public static IEnumerable<Vector3> RasterColliderXYCount(Collider c, int countX, int countY, float border = 1f)
    {
        RaycastHit rayHit;

        var b = c.bounds;

        for (int rx = 0; rx < countX; ++rx)
        {
            for (int ry = 0; ry < countY; ++ry)
            {
                float x = UKMathHelper.MapIntoRange(rx, 0, countX - 1, b.min.x - border, b.max.x + border);
                float y = UKMathHelper.MapIntoRange(ry, 0, countY - 1, b.min.y - border, b.max.y + border);

                Ray ray = new Ray(new Vector3(x, y, b.min.z - border), new Vector3(0f, 0f, 1f));

                if (c.Raycast(ray, out rayHit, b.size.z + border * 2f))
                {
                    yield return rayHit.point;
                }
            }
        }
    }

    /// <summary>
    /// Rasterize a collider by equal distributed raycasts with stepSize between each rays
    /// </summary>
    /// <param name="c"></param>
    /// <param name="stepSize"></param>
    /// <param name="border"></param>
    /// <returns></returns>
    public static IEnumerable<Vector3> RasterColliderXYStep(Collider c, float stepSize, float border = 1f)
    {
        RaycastHit rayHit;

        var b = c.bounds;

        foreach (var x in UKMathHelper.EnumValuesBetween(b.min.x - border, b.max.x + border, stepSize))
        {
            foreach (var y in UKMathHelper.EnumValuesBetween(b.min.y - border, b.max.y + border, stepSize))
            {
                Ray ray = new Ray(new Vector3(x, y, b.min.z - border), new Vector3(0f, 0f, 1f));

                if (c.Raycast(ray, out rayHit, b.size.z + border * 2f))
                {
                    yield return rayHit.point;
                }
            }
        }
    }

    /// <summary>
    /// Rasterize a collider by count randomly distributed raycasts
    /// </summary>
    /// <param name="c"></param>
    /// <param name="count"></param>
    /// <param name="border"></param>
    /// <returns></returns>
    public static IEnumerable<Vector3> RasterColliderXYRandom(Collider c, int count, float border = 1f)
    {
        RaycastHit rayHit;

        var b = c.bounds;

        for (int i = 0; i < count; ++i)
        {
            float x = UKMathHelper.MapIntoRange(UnityEngine.Random.value, 0, 1, b.min.x - border, b.max.x + border);
            float y = UKMathHelper.MapIntoRange(UnityEngine.Random.value, 0, 1, b.min.y - border, b.max.y + border);

            Ray ray = new Ray(new Vector3(x, y, b.min.z - border), new Vector3(0f, 0f, 1f));

            if (c.Raycast(ray, out rayHit, b.size.z + border * 2f))
            {
                yield return rayHit.point;
            }
        }
    }
}
