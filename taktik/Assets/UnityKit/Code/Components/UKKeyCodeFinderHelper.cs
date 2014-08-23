using UnityEngine;
using System.Collections;
using System.Linq;

public class UKKeyCodeFinderHelper : MonoBehaviour {
	void Update () {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>())
            {
                if (Input.GetKey(k))
                {
                    Debug.Log(k);
                }
            }
        }
	}
}
