using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;

public class Network : UKUnitySingletonManuallyCreated<Network> {
    public Player Player0;
    public Player Player1;

    public Server ServerPlayer0;
    public Server ServerPlayer1;

    void OnEnable()
    {
        ServerPlayer0.EvReceiveMessage += ServerPlayer0_EvReceiveMessage;
        ServerPlayer1.EvReceiveMessage += ServerPlayer1_EvReceiveMessage;
    }

    void OnDisable()
    {
        ServerPlayer0.EvReceiveMessage -= ServerPlayer0_EvReceiveMessage;
        ServerPlayer1.EvReceiveMessage -= ServerPlayer1_EvReceiveMessage;
    }

    private void ServerPlayer1_EvReceiveMessage(string message)
    {
        RecieveMessage(1, message);
    }

    private void ServerPlayer0_EvReceiveMessage(string message)
    {
        RecieveMessage(0, message);
    }

    private void RecieveMessage(int playerId, string message)
    {
        // {res: 0-2}
        JSONObject json = JSONObject.Parse(message.Trim());

        //Debug.LogWarning(string.Format("id {0} t {1}", playerId, message));

        Unit.UnitType? t = null;
        if (json != null && json.ContainsKey("res"))
        {
            t = (Unit.UnitType)(int)(json.GetNumber("res"));
        }

        if (t.HasValue)
        {
            if (playerId == 0)
            {
                Player0.AddUnitToPool(t.Value, true);
            }
            else if (playerId == 1)
            {
                Player1.AddUnitToPool(t.Value, true);
            }
        }
    }


    /*
	// Use this for initialization
	IEnumerator Start () {
        while(true)
        {
            yield return new WaitForSeconds(3f);
            Player0.AddUnitToPool((Unit.UnitType)Random.Range(0, 3));
            Player1.AddUnitToPool((Unit.UnitType)Random.Range(0, 3));
        }
	}
	*/

	// Update is called once per frame
	void Update () {
	
	}
}
