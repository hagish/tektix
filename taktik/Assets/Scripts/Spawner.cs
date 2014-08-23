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
 
    public Unit Spawn(Unit.UnitType type)
    {
        foreach(var unit in Prefabs)
        {
            if (unit.Type == type)
            {
                return (GameObject.Instantiate(unit.gameObject) as GameObject).GetComponent<Unit>();
            }
        }

        Debug.LogError(string.Format("invalid unit type {0}", type), gameObject);

        return null;
    }
}
