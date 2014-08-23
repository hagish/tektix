using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlotManager : MonoBehaviour {
    public int PlayerId;
    public List<Slot> Slots;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AddUnit(Unit unit)
    {
        foreach(var slot in Slots)
        {
            if (slot.IsFree)
            {
                slot.AddUnit(unit);
                return;
            }
        }
    }
}
