using UnityEngine;
using System.Collections;

public class GameCore : MonoBehaviour {
    public float RoundTime;
    
    public int RoundIndex;

    public Player Player0;
    public Player Player1;

	// Use this for initialization
	IEnumerator Start () {
	    while(true)
        {
            yield return StartCoroutine(CoCountDown(RoundTime));
            yield return StartCoroutine(CoCalculateRound());
            ++RoundIndex;
        }

        yield return null;
	}

    IEnumerator CoCalculateRound()
    {
        Debug.Log("calculate round", gameObject);

        int count = Mathf.Min(Player0.SlotManager.Slots.Count, Player1.SlotManager.Slots.Count);

        for (int i = 0; i < count; ++i)
        {
            Unit unit0 = Player0.SlotManager.Slots[i].Unit;
            Unit unit1 = Player1.SlotManager.Slots[i].Unit;

            // calculate score
            int winner = CalculateWinner(unit0, unit1);
            if (winner == 0) Player0.Score += 1;
            if (winner == 1) Player1.Score += 1;

            Player0.SlotManager.Slots[i].Clear();
            Player1.SlotManager.Slots[i].Clear();
        }
        
        yield return null;
    }

    private bool Beats(Unit.UnitType a, Unit.UnitType b)
    {
        return (a == Unit.UnitType.PAPER && b == Unit.UnitType.ROCK) ||
            (a == Unit.UnitType.ROCK && b == Unit.UnitType.SCISSOR) ||
            (a == Unit.UnitType.SCISSOR && b == Unit.UnitType.PAPER);
    }

    private int CalculateWinner(Unit unit0, Unit unit1)
    {
        // empty
        if (unit0 == null && unit1 == null) return -1;
        if (unit0 == null && unit1 != null) return 0;
        if (unit0 != null && unit1 == null) return 1;
        // totally complex mechanic
        var t0 = unit0.Type;
        var t1 = unit1.Type;
        if (Beats(t0, t1)) return 0;
        if (Beats(t1, t0)) return 1;
        return -1;
    }

    IEnumerator CoCountDown(float time)
    {
        while(time > 0f)
        {
            Debug.Log(string.Format("time {0}", time), gameObject);
            yield return new WaitForSeconds(1f);
            time -= 1f;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
