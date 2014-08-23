using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class UKGameObjectExtension {
	/// <summary>
	/// Checks if an object has a given object as parent.
	/// </summary>
	/// <returns><c>true</c> if parent is child's parent or if both are  equal.
	/// <param name="child">Child.</param>
	/// <param name="parent">Parent.</param>
	public static bool IsChildOfParent(this GameObject child, GameObject parent)
	{
		if (child == parent)
		{
			return true;
		}
		else if (child.transform.parent != null)
		{
			return IsChildOfParent(child.transform.parent.gameObject, parent);
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// Finds the first child object with a given name (depth first).
	/// </summary>
	/// <returns>The child with the name or null.</returns>
	/// <param name="root">Root.</param>
	/// <param name="name">Name.</param>
	public static GameObject FindChildByNameDeep(this GameObject root, string name){
	        if (root.name == name){
	                return root;    
	        }
	        else {
	                int count = root.transform.childCount;
	                
	                for (int i = 0; i < count; ++i){
	                        GameObject child = root.transform.GetChild(i).gameObject;
	                        
	                        GameObject searchResult = FindChildByNameDeep(child, name);
	                        
	                        if (searchResult != null){
	                                return searchResult;
	                        }
	                }
	        }
	        
	        return null;
	}

	/// <summary>
	/// Finds the first child with a given substring in its name.
	/// </summary>
	/// <returns>The first child or null.</returns>
	/// <param name="root">Root.</param>
	/// <param name="substring">Substring.</param>
	public static GameObject FindChildBySubstringInName(this GameObject root, string substring){
		if (root.name.IndexOf(substring) != -1){
			return root;	
		}
		else {
			int count = root.transform.childCount;
			
			for (int i = 0; i < count; ++i){
				GameObject child = root.transform.GetChild(i).gameObject;
				
				GameObject searchResult = FindChildBySubstringInName(child, substring);
				
				if (searchResult != null){
					return searchResult;
				}
			}
		}
		
		return null;
	}
	
	public static void VisitComponentsInDirectChildren<T>(this GameObject root, Action<T> callback) where T : Component {
		int count = root.transform.childCount;
		
		for (int i = 0; i < count; ++i){
			GameObject child = root.transform.GetChild(i).gameObject;
			
			T t = child.GetComponent<T>();
			if (t) callback(t);
		}
	}
	
	public static IEnumerable<GameObject> EnumGameObjectsDeep(this GameObject root) {
		yield return root;
		
		int count = root.transform.childCount;
		
		for (int i = 0; i < count; ++i){
			GameObject child = root.transform.GetChild(i).gameObject;
			
			foreach (var x in child.EnumGameObjectsDeep())
			{
			 	yield return x;
			}
		}
	}
		
	public static IEnumerable<GameObject> EnumChilds(this GameObject gameObject)
	{
 		int count = gameObject.transform.childCount;
	                
        for (int i = 0; i < count; ++i){
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            yield return child;
        }		
	}
	
	public static IEnumerable<T> EnumComponentsDeep<T>(this GameObject root) where T : Component {
		T t = root.GetComponent<T>();
		if (t) yield return t;
		
		int count = root.transform.childCount;
		
		for (int i = 0; i < count; ++i){
			GameObject child = root.transform.GetChild(i).gameObject;
			
			foreach (var x in child.EnumComponentsDeep<T>())
			{
			 	yield return x;
			}
		}
	}
		
	/**
	 * recursive and expensive
	 */
	public static void VisitComponentsDeep<T>(this GameObject root, Action<T> callback) where T : Component {
		foreach(var c in root.EnumComponentsDeep<T>())
		{
			callback(c);
		}
	}
	
	public static void VisitGameObjectsDeep(this GameObject root, Action<GameObject> callback)
	{
		callback(root);
		
		int count = root.transform.childCount;
		
		for (int i = 0; i < count; ++i){
			GameObject child = root.transform.GetChild(i).gameObject;
			
			VisitGameObjectsDeep(child, callback);
		}			
	}
	
	public static void RemoveAllComponentsOfType<T>(this GameObject root) where T : Component {
		T[] ts = root.GetComponentsInChildren<T>();
		
		foreach(T t in ts)
		{
			Component.Destroy(t);
		}
	}
	
	public static bool HasParentWithName(this GameObject child, String parentName)
	{
		if (child.name == parentName)
		{
			return true;	
		}
		
		if (child.transform.parent != null)
		{
			return HasParentWithName(child.transform.parent.gameObject, parentName);	
		}
		else
		{
			return false;
		}
	}
	
	public static String PrintPathToRoot(this GameObject o)
	{
		if (o)
		{
			String name = "?";
			
			if (o.name.Length > 0)
			{
				name = o.name;
			}
			
			if (o.transform.parent)
			{
				return PrintPathToRoot(o.transform.parent.gameObject) + "." + name;
			}
			else
			{
				return name;	
			}
		}
		else
		{
			return "";
		}
	}
	
	public static Vector3 GetAABBMeshCenter(this GameObject o)
	{
		MeshFilter mf = o.GetComponent<MeshFilter>();
		if (mf != null)
		{
			return o.transform.TransformPoint(mf.mesh.bounds.center);
		}
		
		throw new Exception("there is no mesh to calculate center");
	}

	public static void RemoveComponent<T>(this GameObject gameObject) where T : Component
	{
		T t = gameObject.GetComponent<T>();
		if (t != null)
		{
			GameObject.Destroy(t);
		}
	}
	
	public static T EnsureComponent<T>(this GameObject gameObject) where T : Component
	{
		T t = gameObject.GetComponent<T>();
		if (t == null)
		{
			t = gameObject.AddComponent<T>();
		}
		return t;
	}

	public static T FindComponentDeep<T>(this GameObject gameObject) where T : Component
	{
		foreach(var c in gameObject.EnumComponentsDeep<T>())
		{
			return c;
		}
		
		return null;
	}
	
	public static T FindComponentUpwards<T>(this GameObject gameObject) where T : Component
	{
		Transform t = gameObject.transform;

		while (t) {
			var c = t.GetComponent<T>();
			if (c != null) return c;

			t = t.transform.parent;
		}

		return null;
	}
	
	public static void RemoveAllChildren(this GameObject gameObject)
	{
		int counter = gameObject.transform.childCount;
		while( counter > 0 )
		{
			counter--;
			GameObject.Destroy( gameObject.transform.GetChild(counter).gameObject );
		}
	}

	public static void ResetTransformLocal(this GameObject gameObject)
	{
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localRotation = Quaternion.identity;
	}
}
