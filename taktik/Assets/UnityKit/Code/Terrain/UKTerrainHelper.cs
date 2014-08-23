using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class UKTerrainHelper {

	public static string[] GetTerrainSplatNames(TerrainData d) {
		List<string> l = new List<string>();
		
		for (int i = 0; i < d.splatPrototypes.Length; ++i) {
			l.Add(d.splatPrototypes[i].texture.name);
		}
		
		return l.ToArray();
	}
	
	public static string GetMaxTerrainSplatName(TerrainData d, int x, int y, float[,,] alphas) {
		int l = alphas.GetLength(2);
		
		int maxIndex = 0;
		float maxValue = 0;
		
		for (int i = 0; i < l; ++i) {
			if (alphas[x,y,i] > maxValue) {
				maxIndex = i;
				maxValue = alphas[x,y,i];
			}
		}
		
		return d.splatPrototypes[maxIndex].texture.name;
	}
	
	public static string GetTerrainMainTextureAt(Terrain t, Vector3 worldPos) {
		TerrainData d = t.terrainData;
		var local = worldPos - t.transform.position;
		var rel = new Vector3(local.x / d.size.x, 0f, local.z / d.size.z);
		int px = Mathf.FloorToInt(d.alphamapWidth * rel.x);
		int py = Mathf.FloorToInt(d.alphamapHeight * rel.z);
		if (px < 0 || px >= d.alphamapWidth || py < 0 || py >= d.alphamapHeight) {
			//Debug.Log(string.Format("{0} {1}", px, py));
			throw new UnityException("out of terrain area");
		}
		//Debug.Log(string.Format("local={0} rel={1} p={2}/{3}", local, rel, px, py));
		var alphas = d.GetAlphamaps(px, py, 1, 1);
		return GetMaxTerrainSplatName(d, 0, 0, alphas);
	}
}
