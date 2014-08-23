using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public int Id;
    public SlotManager Lanes;
    public SlotManager UnitPool;
    public int Score;
    private Slot selectedUnitPoolSlot;
    public GameObject Selection;
    public Vector3 SpawnVelocity;
    public Vector3 SpawnRotation;

    void Start()
    {
        AddUnitToPool(Unit.UnitType.PAPER);
        AddUnitToPool(Unit.UnitType.ROCK);
        AddUnitToPool(Unit.UnitType.SCISSOR);
        AddUnitToPool(Unit.UnitType.PAPER);
        AddUnitToPool(Unit.UnitType.SCISSOR);
    }

    public void AddUnitToPool(Unit.UnitType type)
    {
        if (UnitPool.HasFreeSlots)
        {
            var unit = Spawner.Instance.Spawn(type);
            Debug.Log(string.Format("added unit {0} to pool", unit), gameObject);
            UnitPool.AddUnitAnywhere(unit);
        }
        else
        {
            Debug.Log("no free index slot in pool -> drop");
        }
    }

    void OnClickPoolSlot(GameObject target)
    {
        selectedUnitPoolSlot = target.GetComponent<Slot>();
    }

    void OnClickLane(GameObject target)
    {
        Slot selectedLane = target.GetComponent<Slot>();

        if (selectedUnitPoolSlot != null && !selectedUnitPoolSlot.IsFree && selectedLane != null && selectedLane.IsFree)
        {
            var unit = selectedUnitPoolSlot.Unit;
            selectedUnitPoolSlot.Unit = null;
            //selectedLane.AddUnit(unit);
            unit.transform.position = selectedLane.transform.position;
            unit.gameObject.AddComponent<UnitMovement>().Velocity = SpawnVelocity;
            unit.transform.rotation = Quaternion.Euler(SpawnRotation);
            unit.PlayerId = Id;
        }
    }

    void Update()
    {
        var active = selectedUnitPoolSlot != null && !selectedUnitPoolSlot.IsFree;
        Selection.SetActive(active);
        if (active)
        {
            Selection.transform.position = selectedUnitPoolSlot.transform.position;
        }
    }
}
