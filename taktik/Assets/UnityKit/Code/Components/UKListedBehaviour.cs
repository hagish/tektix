using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// All instances are automatically managed in a list.
/// ATTENTION If you overwrite Awake or OnDestroy you must call the base class methods. 
/// </summary>
public class UKListedBehaviour<T> : MonoBehaviour
	where T : Component
{
	public static UKList<GameObject> _instances = new UKList<GameObject>();

	/// <summary>
	/// Gets all active instances.
	/// </summary>
	/// <value>The instances.</value>
	public static IEnumerable<T> Instances {
		get {
			foreach(GameObject it in _instances) if (it != null && it.activeSelf) yield return it.GetComponent<T>();
		}
	}

	/// <summary>
	/// Gets all instances including inactive ones.
	/// </summary>
	/// <value>The instances all.</value>
	public static IEnumerable<T> InstancesAll {
		get {
			foreach(GameObject it in _instances) if (it != null) yield return it.GetComponent<T>();
		}
	}

	public virtual void Awake () {
		_instances.Add (this.gameObject);
	}

	public virtual void OnDestroy () {
		_instances.Remove (this.gameObject);
	}
}