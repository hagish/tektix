using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlotManager : MonoBehaviour {
    public int PlayerId;
    public List<Slot> Slots;

    public bool HasFreeSlots
    {
        get
        {
            foreach (var slot in Slots)
            {
                if (slot.IsFree)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public void AddUnitAnywhere(Unit unit)
    {
        foreach (var slot in Slots)
        {
            if (slot.IsFree)
            {
                slot.AddUnit(unit);
                return;
            }
        }
    }

    public void AddUnit(Unit unit, int laneId)
    {
        foreach(var slot in Slots)
        {
            if (slot.IsFree && slot.LaneId == laneId)
            {
                slot.AddUnit(unit);
                return;
            }
        }
    }

    public Unit PopUnitFromSlot(int index)
    {
        if (Slots[index].IsFree)
        {
            Debug.LogError(string.Format("no unit at index {0}", index), gameObject);
            return null;
        }
        else
        {
            Unit u = Slots[index].Unit;
            Slots[index].Unit = null;
            return u;
        }
    }

}
