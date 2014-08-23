// uncomment this for logging
#define LOG_ADD_LISTENER
#define LOG_BROADCAST_MESSAGE
#define LOG_BROADCAST_WITHOUT_RECIPIENT

using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Global message bus system. Each listener is linked to a GameObject and gets removed if the gameobject dies.
 **/
public static class UKMessenger {

	struct ListenerLifecyclePair {
		public Delegate Listener;
		public GameObject LifecycleObject;
	}

	class EventListeners {
		public UKList<ListenerLifecyclePair> Listeners = new UKList<ListenerLifecyclePair>();
	}

	private static Dictionary<string, EventListeners> eventTable = new Dictionary<string, EventListeners>();

	// sends a message
	public static void Broadcast(string messageName) {
		#if LOG_BROADCAST_MESSAGE
		Debug.Log(string.Format("MESSENGER send message {0} ()", messageName));
		#endif

		EventListeners l;
		if (eventTable.TryGetValue(messageName, out l)) {
			int countDead = 0;

			var listeners = l.Listeners;

            #if LOG_BROADCAST_WITHOUT_RECIPIENT
            if (listeners.Count == 0)
            {
                Debug.LogWarning(string.Format("MESSENGER send message {0} () without recipient", messageName));
            }
		    #endif            

			for (int i = 0; i < listeners.Count; ++i) {
				if (listeners[i].LifecycleObject != null) {
					// listener still alive -> try to call
					UKCallback callback = listeners[i].Listener as UKCallback;
					
					if (callback != null) {
						callback();
					}
				} else {
					++countDead;
				}
			}

			if (countDead > 0) {
				PurgeDeadListeners(l);
			}
		}
	}

	// sends a message
	public static void Broadcast<T0>(string messageName, T0 p0) {
		#if LOG_BROADCAST_MESSAGE
		Debug.Log(string.Format("MESSENGER send message {0} ({1})", messageName, p0));
		#endif

		EventListeners l;
		if (eventTable.TryGetValue(messageName, out l)) {
			int countDead = 0;
			
			var listeners = l.Listeners;

            #if LOG_BROADCAST_WITHOUT_RECIPIENT
            if (listeners.Count == 0)
            {
                Debug.LogWarning(string.Format("MESSENGER send message {0} ({1})  without recipient", messageName, p0)); 
            }
            #endif            
			
			for (int i = 0; i < listeners.Count; ++i) {
				if (listeners[i].LifecycleObject != null) {
					// listener still alive -> try to call
					UKCallback<T0> callback = listeners[i].Listener as UKCallback<T0>;

					if (callback != null) {
						callback(p0);
					}
				} else {
					++countDead;
				}
			}
			
			if (countDead > 0) {
				PurgeDeadListeners(l);
			}
		}
	}

	// sends a message
	public static void Broadcast<T0, T1>(string messageName, T0 p0, T1 p1) {
		#if LOG_BROADCAST_MESSAGE
		Debug.Log(string.Format("MESSENGER send message {0} ({1}, {2})", messageName, p0, p1));
		#endif

		EventListeners l;
		if (eventTable.TryGetValue(messageName, out l)) {
			int countDead = 0;
			
			var listeners = l.Listeners;

            #if LOG_BROADCAST_WITHOUT_RECIPIENT
            if (listeners.Count == 0)
            {
                Debug.LogWarning(string.Format("MESSENGER send message {0} ({1}, {2}) without recipient", messageName, p0, p1)); 
            }
            #endif        

			for (int i = 0; i < listeners.Count; ++i) {
				if (listeners[i].LifecycleObject != null) {
					// listener still alive -> try to call
					UKCallback<T0, T1> callback = listeners[i].Listener as UKCallback<T0, T1>;
					
					if (callback != null) {
						callback(p0, p1);
					}
				} else {
					++countDead;
				}
			}
			
			if (countDead > 0) {
				PurgeDeadListeners(l);
			}
		}
	}

	// sends a message
	public static void Broadcast<T0, T1, T2>(string messageName, T0 p0, T1 p1, T2 p2) {
		#if LOG_BROADCAST_MESSAGE
		Debug.Log(string.Format("MESSENGER send message {0} ({1}, {2}, {3})", messageName, p0, p1, p2));
		#endif

		EventListeners l;
		if (eventTable.TryGetValue(messageName, out l)) {
			int countDead = 0;
			
			var listeners = l.Listeners;
			
            #if LOG_BROADCAST_WITHOUT_RECIPIENT
            if (listeners.Count == 0)
            {
                Debug.LogWarning(string.Format("MESSENGER send message {0} ({1}, {2}, {3}) without recipient", messageName, p0, p1, p2));
            }
            #endif       
            
            for (int i = 0; i < listeners.Count; ++i) {
				if (listeners[i].LifecycleObject != null) {
					// listener still alive -> try to call					
					UKCallback<T0, T1, T2> callback = listeners[i].Listener as UKCallback<T0, T1, T2>;
					
					if (callback != null) {
						callback(p0, p1, p2);
					}
				} else {
					++countDead;
				}
			}
			
			if (countDead > 0) {
				PurgeDeadListeners(l);
			}
		}
	}


	private static void PurgeDeadListeners(EventListeners l) {
		var listeners = l.Listeners;
		for (int i = 0; i < listeners.Count; ++i) {
			if (listeners[i].LifecycleObject == null) {
				// dead listener found -> remove
				listeners.RemoveAt(i);
				--i;
			}
		}
	}

	private static void AddListenerInternal(string messageName, GameObject lifecycleObject, Delegate handler) {
		#if LOG_ADD_LISTENER
		Debug.Log(string.Format("MESSENGER add listener {0} at {1}", handler, messageName), lifecycleObject);
		#endif

		// don't add dead listeners
		if (lifecycleObject == null) return;

		// prepare
		if (!eventTable.ContainsKey(messageName)) {
			eventTable[messageName] = new EventListeners();
		}
		
		// add
		eventTable[messageName].Listeners.Add(new ListenerLifecyclePair(){
			Listener = handler,
			LifecycleObject = lifecycleObject,
		});
	}

	// adds an listener linked to the lifetime of a gameobject
	public static void AddListener(string messageName, GameObject lifecycleObject, UKCallback handler) {
		AddListenerInternal(messageName, lifecycleObject, handler);
	}

	public static void AddListener<T0>(string messageName, GameObject lifecycleObject, UKCallback<T0> handler) {
		AddListenerInternal(messageName, lifecycleObject, handler);
	}

	public static void AddListener<T0, T1>(string messageName, GameObject lifecycleObject, UKCallback<T0, T1> handler) {
		AddListenerInternal(messageName, lifecycleObject, handler);
	}
	
	public static void AddListener<T0, T1, T2>(string messageName, GameObject lifecycleObject, UKCallback<T0, T1, T2> handler) {
		AddListenerInternal(messageName, lifecycleObject, handler);
	}	

	// removes all listener
	public static void Cleanup() {
		eventTable.Clear();
	}
}
