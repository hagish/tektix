using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UKObjectRecycler : MonoBehaviour {
	public static UKObjectRecycler Instance;

	public static void DepositObject(GameObject o) 
	{
		o.GetComponent<UKObjectRecyclerDepositMe>().Deposit();
	}

	public static void DepositObject(GameObject o, float timeout) 
	{
		o.GetComponent<UKObjectRecyclerDepositMe>().Deposit(timeout);
	}

	private Dictionary<string, UKQueue<GameObject>> cachedObjects = new Dictionary<string, UKQueue<GameObject>>();
	
	void Awake()
	{
		Instance = this;
	}
	
	private UKQueue<GameObject> GetQueueByGroup(string recycleGroup)
	{
		if (!cachedObjects.ContainsKey (recycleGroup)) {
			cachedObjects [recycleGroup] = new UKQueue<GameObject> ();
		}
		
		return cachedObjects[recycleGroup];
	}
	
	private GameObject PopObjectFromQueueByGroup(string recycleGroup)
	{
		var queue = GetQueueByGroup(recycleGroup);
		
		while(queue.Count > 0)
		{
			GameObject o = queue.Dequeue();
			if (o != null)return o;
		}
		
		return null;
	}

	// uses the prefab name as recycle group
	public GameObject GetObject(GameObject prefab)
	{
		return GetObject(prefab.name, prefab);
	}

	public GameObject GetObject(string recycleGroup, GameObject prefab)
	{
		GameObject o = PopObjectFromQueueByGroup(recycleGroup);
		if (o != null)
		{
			// recycle
			o.SetActive(true);
			o.SendMessage("OnRestart", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			// create new
			o = (GameObject)GameObject.Instantiate(prefab);
			if (o.GetComponent<UKObjectRecyclerDepositMe>() == null) {
				var dm = o.AddComponent<UKObjectRecyclerDepositMe>();
				dm.recycler = this;
				dm.recyclerGroup = recycleGroup;
			}
		}
		
		return o;
	}
	
	public GameObject GetObject(string recycleGroup, System.Func<GameObject> factory)
	{
		GameObject o = PopObjectFromQueueByGroup(recycleGroup);
		if (o != null)
		{
			// recycle
			o.SetActive(true);
			o.SendMessage("OnRestart", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			// create new
			o = factory();			
			if (o.GetComponent<UKObjectRecyclerDepositMe>() == null) {
				var dm = o.AddComponent<UKObjectRecyclerDepositMe>();
				dm.recycler = this;
				dm.recyclerGroup = recycleGroup;
			}
		}
		
		return o;
	}
	
	public GameObject GetObjectAt(string recycleGroup, Vector3 pos, Quaternion rot, System.Func<GameObject> factory)
	{
		GameObject o = GetObject(recycleGroup, factory);
		o.transform.position = pos;
		o.transform.rotation = rot;
		return o;
	}

	public GameObject GetObjectAt(string recycleGroup, Vector3 pos, Quaternion rot, GameObject prefab)
	{
		return GetObjectAt(recycleGroup, pos, rot, () => {
			return (GameObject)GameObject.Instantiate(prefab);
		});
	}
	
	public void DepositObject(string recycleGroup, GameObject o)
	{
		o.SendMessage("OnDeposit", SendMessageOptions.DontRequireReceiver);
		o.SetActive(false);
		GetQueueByGroup(recycleGroup).Enqueue(o);
	}
	
	public IEnumerable<GameObject> EnumAllByGroup(string recycleGroup)
	{
		foreach(var o in GetQueueByGroup(recycleGroup))
		{
			yield return o;
		}
	}
	
	public IEnumerable<GameObject> EnumAll()
	{
		foreach(var t in cachedObjects.Keys)
		foreach(var o in GetQueueByGroup(t))
		{
			yield return o;
		}
	}
}
