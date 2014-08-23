using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {
    public enum UnitType {
        SCISSOR = 0,
        ROCK = 1,
        PAPER = 2,
    }

    public int PlayerId;

    public UnitType Type;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
