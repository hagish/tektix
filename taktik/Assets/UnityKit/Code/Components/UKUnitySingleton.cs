using UnityEngine;

/// <summary>
/// Singleton, automatically created.
/// http://redframe-game.com/blog/global-managers-with-generic-singletons/
/// </summary>
public class UKUnitySingleton<T> : MonoBehaviour
	where T : Component
{
	private static T _instance;
	public static T Instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType (typeof(T)) as T;
				if (_instance == null) {
					GameObject obj = new GameObject ();
					obj.hideFlags = HideFlags.HideAndDontSave;
					_instance = obj.AddComponent<T>();
				}
			}
			return _instance;
		}
	}
}