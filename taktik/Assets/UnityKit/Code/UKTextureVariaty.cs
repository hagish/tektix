using UnityEngine;
using System.Collections;

public class UKTextureVariaty : MonoBehaviour {

	public int SegmentCountX;
	public int SegmentCountY;

	public int SegmentX;
	public int SegmentY;

	void Start () 
	{
		SetSegment(SegmentX, SegmentY);
	}

	public void SetSegment(int x, int y) {
		if (SegmentCountX > 0 && SegmentCountY > 0) {

			SegmentX = x % SegmentCountX;
			SegmentY = y % SegmentCountY;

			if (renderer != null) {
				float dx = 1f / (float)SegmentCountX;
				float dy = 1f / (float)SegmentCountY;
				Vector2 off = new Vector2((float)x * dx, (float)y * dy);
				renderer.material.SetTextureOffset("_MainTex", off);
			}

		}
	}
}
