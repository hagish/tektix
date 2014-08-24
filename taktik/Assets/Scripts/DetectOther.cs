using UnityEngine;
using System.Collections;

public class DetectOther : MonoBehaviour {
    private Unit myUnit;

    void Awake()
    {
        myUnit = gameObject.FindComponentUpwards<Unit>();
    }

    void OnTriggerEnter(Collider other)
    {
        var otherUnit = other.gameObject.FindComponentUpwards<Unit>();
        if (otherUnit != null && otherUnit.PlayerId != myUnit.PlayerId && !GameCore.IgnoreEachOther(myUnit, otherUnit))
        {
            SendMessageUpwards("OtherDetectedOn");
        }
    }

    void OnTriggerExit(Collider other)
    {
        var otherUnit = other.gameObject.FindComponentUpwards<Unit>();
        if (otherUnit != null && otherUnit.PlayerId != myUnit.PlayerId && !GameCore.IgnoreEachOther(myUnit, otherUnit))
        {
            SendMessageUpwards("OtherDetectedOff");
        } 
    }

}
