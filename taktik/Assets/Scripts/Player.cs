using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public int Id;
    public SlotManager SlotManager;
    public int Score;
    public int SlotCount;
    public int SelectedLaneId;

    private List<Unit> unitsPool = new List<Unit>();

    public List<Unit> PrefabUnits;

    void Start()
    {
        AddUnitToPool(Unit.UnitType.PAPER);
        AddUnitToPool(Unit.UnitType.ROCK);
        AddUnitToPool(Unit.UnitType.SCISSOR);
        AddUnitToPool(Unit.UnitType.PAPER);
        AddUnitToPool(Unit.UnitType.SCISSOR);
    }

    public void SelectUnitFromPool(int index)
    {
        Debug.Log(string.Format("player {0} selects {1}", Id, index), gameObject);

        if (unitsPool[index] != null)
        {
            var unit = PopUnitFromPool(index);
            SlotManager.AddUnit(unit, SelectedLaneId);
        }
    }

    public Unit PopUnitFromPool(int index)
    {
        if (unitsPool[index] == null)
        {
            Debug.LogError(string.Format("no unit at index {0}", index), gameObject);
            return null;
        }
        else
        {
            Unit u = unitsPool[index];
            unitsPool[index] = null;
            return u;
        }
    }

    public void SelectLane(int laneId)
    {
        SelectedLaneId = laneId;
    }

    public void AddUnitToPool(Unit.UnitType type)
    {
        for (int i = 0; i < SlotCount; ++i)
        {
            if (i < unitsPool.Count && unitsPool[i] == null)
            {
                var unit = Spawner.Instance.Spawn(type);
                Debug.Log(string.Format("added unit {0} to index {1}", unit, i), gameObject);
                unitsPool[i] = unit;
                return;
            }
            else if (unitsPool.Count < SlotCount)
            {
                var unit = Spawner.Instance.Spawn(type);
                Debug.Log(string.Format("added unit {0} to index {1}", unit, i), gameObject);
                unitsPool.Add(unit);
                return;
            }
        }

        Debug.Log("no free index slot in pool -> drop");
    }
}
