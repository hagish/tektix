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
    public int InitialPieces = 3;
    public Unit.UnitType? PreferredType;
    public float Difficulty = 1;
    public float SlotAddTimeout = 1f;

    void Start()
    {
        for (int i = 0; i < InitialPieces; ++i)
        {
            AddUnitToPool((Unit.UnitType)(int)(Random.Range(0,3)), false);
        }

        foreach(var ps in Selection.gameObject.EnumComponentsDeep<ParticleSystem>())
        {
            ps.Simulate(5f);
        }
    }

    public void AddUnitToPool(Unit.UnitType type, bool playSound)
    {
        if (!UnitPool.HasFreeSlots)
        {
            UnitPool.DestroyOldestUnit();
        }

        var unit = Spawner.Instance.Spawn(type, Id);
        //Debug.Log(string.Format("added unit {0} to pool", unit), gameObject);
        UnitPool.AddUnitAnywhere(unit);
        unit.transform.rotation = Quaternion.Euler(SpawnRotation);

        if (playSound) AudioController.Instance.PlaySound("new_item");

        UpdateBlinking();
    }

    void OnClickPoolSlot(GameObject target)
    {
        //Debug.Log("OnClickPoolSlot", gameObject);
        selectedUnitPoolSlot = target.FindComponentUpwards<Slot>();
        AudioController.Instance.PlaySound("click");
    }

    void OnClickLane(GameObject target)
    {
        Slot selectedLane = target.FindComponentUpwards<Slot>();

        if (selectedUnitPoolSlot != null && !selectedUnitPoolSlot.IsFree && selectedLane != null && selectedLane.IsFree)
        {
            if (Time.time - selectedLane.LastAddTime > SlotAddTimeout)
            {
                var unit = selectedUnitPoolSlot.Unit;
                selectedUnitPoolSlot.Unit = null;
                selectedUnitPoolSlot = null;
                selectedLane.LastAddTime = Time.time;
                unit.transform.position = selectedLane.transform.position;
                unit.gameObject.AddComponent<UnitMovement>().Velocity = SpawnVelocity;
                unit.transform.rotation = Quaternion.Euler(SpawnRotation);
                unit.PlayerId = Id;
                unit.BroadcastMessage("StopBlinking");
                AudioController.Instance.PlaySound("click");

                UpdateBlinking();
            }
        }
    }

    void UpdateBlinking()
    {
        foreach (var slot in UnitPool.Slots)
        {
            if (slot.Unit != null) slot.Unit.BroadcastMessage("StopBlinking");
        } 
        
        if (!UnitPool.HasFreeSlots)
        {
            UnitPool.OldestUnit().BroadcastMessage("StartBlinking");
        }
    }

    void Update()
    {
        var active = selectedUnitPoolSlot != null && !selectedUnitPoolSlot.IsFree;
        if (active)
        {
            Selection.transform.position = selectedUnitPoolSlot.transform.position;
        }
        else
        {
            Selection.transform.position = new Vector3(10000f, 10000f, 10000f);
        }
    }

    public void SetPreferredType(int type)
    {
        PreferredType = (Unit.UnitType)type;
    }
}
