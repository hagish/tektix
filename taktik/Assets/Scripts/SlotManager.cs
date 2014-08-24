using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SlotManager : MonoBehaviour {
    public int PlayerId;
    public List<Slot> Slots;
    private int nextAddOrder = 0;

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
                slot.AddOrder = nextAddOrder;
                ++nextAddOrder;
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
                slot.AddOrder = nextAddOrder;
                ++nextAddOrder;
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

    public Unit OldestUnit()
    {
        var slot = Slots.OrderBy(it => it.AddOrder).FirstOrDefault();
        if (slot != null) return slot.Unit;
        else return null;
    }

    public void DestroyOldestUnit()
    {
        var slot = Slots.OrderBy(it => it.AddOrder).First();
        Unit u = slot.Unit;
        slot.Unit = null;
        u.Kill(false);
    }
}
