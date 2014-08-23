using UnityEngine;
using System.Collections;

public class Network : MonoBehaviour {
    public Player Player0;
    public Player Player1;

	// Use this for initialization
	IEnumerator Start () {
        while(true)
        {
            yield return new WaitForSeconds(3f);
            Player0.AddUnitToPool((Unit.UnitType)Random.Range(0, 3));
            Player1.AddUnitToPool((Unit.UnitType)Random.Range(0, 3));
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
