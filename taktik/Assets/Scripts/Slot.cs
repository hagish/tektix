﻿using UnityEngine;
using System.Collections;

public class Slot : MonoBehaviour {
    public int LaneId;
    public Unit Unit;

    public float LastAddTime = 0f;

    public int AddOrder;

    public bool IsFree {
        get
        {
            return Unit == null;
        }
    }

    public void AddUnit(Unit unit)
    {
        Unit = unit;
        unit.gameObject.transform.parent = transform;
        unit.gameObject.transform.localPosition = Vector3.zero;
        LastAddTime = Time.time;
    }

    public void Clear()
    {
        if (Unit != null)
        {
            GameObject.Destroy(Unit.gameObject);
            Unit = null;
        }
    }
}
