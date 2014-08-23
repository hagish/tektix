using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 219

public static class UKRandomHelper {
	private static Random r = new Random();
	
	// min/max is included
	public static int Next(int min, int max)
	{
		return min + r.Next() % (Math.Max(max, min) - min + 1);
	}

	// [min,max[
	public static float Next(float min, float max) 
	{
		return min + Next () * (max - min);
	}
	
	// [0-1[
	public static float Next()
	{
		return (float)r.NextDouble();	
	}
	
	/// <summary>
	/// Pick randomly from weighted elements
	/// </summary>
	/// <returns>
	/// A element
	/// </returns>
	/// <param name='itemWeightMap'>
	/// Item weight map, key = items, value = weight (must be int > 0)
	/// </param>
	/// <typeparam name='T'>
	/// Item type
	/// </typeparam>
	public static T PickWeightedRandom<T>(Dictionary<T,int> itemWeightMap)
	{
		int weightSum = 0;
		
		foreach(var pair in itemWeightMap)
		{
			weightSum += pair.Value;
		}
		
		int random = UKRandomHelper.Next(0, weightSum - 1);
		
		foreach(var pair in itemWeightMap)
		{
			int weight = pair.Value;
			if (random < weight)
			{
				return pair.Key;
			}
			
			random -= weight;
		}
		
		throw new Exception("this will not happen");
	}
	
	public static T PickRandom<T>(List<T> items)
	{
		if (items.Count == 0)throw new Exception("you cant pick from an empty list");
		
		int index = Next(0, items.Count - 1);
		return items[index];
	}
	
	public static T PickRandom<T>(IEnumerable<T> items)
	{
		int count = 0;
	
		foreach (T i in items) ++count;

		if (count == 0)throw new Exception("you cant pick from an empty list");
		
		int index = Next(0, count - 1);
		foreach (T i in items) {
			if (index == 0) return i;
			--index;
		}

		throw new Exception("pick random is broken");
	}

	public static T PickWeightedRandom<T>(IEnumerable<T> items, Func<T, int> weightFun)
	{
		var weightMap = new Dictionary<T, int>();
		
		foreach(var item in items)
		{
			weightMap[item] = weightFun(item);
		}
		
		return PickWeightedRandom(weightMap);
	}	
}
