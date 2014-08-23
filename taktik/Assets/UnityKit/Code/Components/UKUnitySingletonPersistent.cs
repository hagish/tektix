using UnityEngine;

/// <summary>
/// Singleton, automatically created. 
/// This instance is persistent and don't get destroyed during scene changes. 
/// http://redframe-game.com/blog/global-managers-with-generic-singletons/
/// ATTENTION If you overwrite Awake you must call the base
/// ATTENTION you can place an object with this in each scene and there is only
/// one global persistent instance and there is only one Start call
/// but there can be multiple Awake calls
/// </summary>
public class UKUnitySingletonPersistent<T> : MonoBehaviour
	where T : Component
{
	protected static T _instance;
	public static T Instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType (typeof(T)) as T;
				if (_instance == null) {
                    GameObject obj = new GameObject(typeof(T).ToString());
                    obj.hideFlags = HideFlags.HideAndDontSave;
					_instance = obj.AddComponent<T> ();
				}
			}
			return _instance;
		}
	}

	public virtual void Awake ()
	{
		if (_instance == null) {
	    	DontDestroyOnLoad (this.gameObject);
			_instance = this as T;
		} else {
			Destroy (gameObject);
		}
	}
}