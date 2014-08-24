using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;

public class Network : UKUnitySingletonManuallyCreated<Network> {
    public Player Player0;
    public Player Player1;

    public Server ServerPlayer0;
    public Server ServerPlayer1;

    public float SendTimeout = 1f;

    void Start()
    {
        InvokeRepeating("SendStatusToAll", SendTimeout, SendTimeout);
    }

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


    void SendStatusToAll()
    {
        JSONObject p0 = CreateJsonStatus(Player0, Player1);
        ServerPlayer0.SendOutgoingMessage(p0.ToString());

        JSONObject p1 = CreateJsonStatus(Player1, Player0);
        ServerPlayer1.SendOutgoingMessage(p1.ToString());
    }

    private JSONObject CreateJsonStatus(Player playerSelf, Player playerOther)
    {
        var j = new JSONObject();

        j.Add("id", playerSelf.Id);
        // score self
        j.Add("self", playerSelf.Score);
        // score other
        j.Add("other", playerOther.Score);
        // preferred color
        if (playerSelf.PreferredType.HasValue) j.Add("wish", (int)playerSelf.PreferredType.Value);
        // difficulty
        j.Add("diff", playerSelf.Difficulty);

        return j;
    }
}
