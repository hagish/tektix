using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {
    public static Spawner Instance;

    void Awake()
    {
        Instance = this;
    }

    public List<Unit> Prefabs;
 
    public Unit Spawn(Unit.UnitType type, int playerId)
    {
        foreach(var unit in Prefabs)
        {
            if (unit.Type == type)
            {
                Unit u = (GameObject.Instantiate(unit.gameObject) as GameObject).GetComponent<Unit>();
                u.PlayerId = playerId;
                return u;
            }
        }

        Debug.LogError(string.Format("invalid unit type {0}", type), gameObject);

        return null;
    }
}
