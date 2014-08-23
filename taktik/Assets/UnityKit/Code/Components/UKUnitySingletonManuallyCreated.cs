using UnityEngine;

/// <summary>
/// Singleton, but you need to manually place one instance in the scene.
/// ATTENTION If you overwrite Awake or OnDestroy you must call the base class methods. 
/// </summary>
public class UKUnitySingletonManuallyCreated<T> : MonoBehaviour
	where T : Component
{
	public static T Instance;

	protected virtual void Awake () {
		Instance = this.GetComponent<T>();
	}

	protected virtual void OnDestroy() {
		Instance = null;
	}
}