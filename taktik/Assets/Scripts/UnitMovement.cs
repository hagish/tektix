using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour {
    public Vector3 Velocity;

	// Update is called once per frame
	void Update () 
    {
        transform.Translate(Time.deltaTime * Velocity, Space.World);
	}
}
