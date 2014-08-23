using UnityEngine;
using System.Collections;

public class GameCore : MonoBehaviour {
    public float RoundTime;
    
    public int RoundIndex;

    public Player Player0;
    public Player Player1;

    private float time;

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

        int count = Mathf.Min(Player0.Lanes.Slots.Count, Player1.Lanes.Slots.Count);

        for (int i = 0; i < count; ++i)
        {
            Unit unit0 = Player0.Lanes.Slots[i].Unit;
            Unit unit1 = Player1.Lanes.Slots[i].Unit;

            // calculate score
            int winner = CalculateWinner(unit0, unit1);
            if (winner == 0) Player0.Score += 1;
            if (winner == 1) Player1.Score += 1;

            UpdateUI();

            Player0.Lanes.Slots[i].Clear();
            Player1.Lanes.Slots[i].Clear();
        }
        
        yield return null;
    }

    private void UpdateUI()
    {
        UIGlue.Instance.Player0Score.text = string.Format("{0:0.} points", Player0.Score);
        UIGlue.Instance.Player1Score.text = string.Format("{0:0.} points", Player1.Score);
        
        UIGlue.Instance.Player0Time.text = string.Format("{0:0.} sec", time);
        UIGlue.Instance.Player1Time.text = string.Format("{0:0.} sec", time);
    }

    public static bool Beats(Unit.UnitType a, Unit.UnitType b)
    {
        return (a == Unit.UnitType.PAPER && b == Unit.UnitType.ROCK) ||
            (a == Unit.UnitType.ROCK && b == Unit.UnitType.SCISSOR) ||
            (a == Unit.UnitType.SCISSOR && b == Unit.UnitType.PAPER);
    }

    public static int CalculateWinner(Unit unit0, Unit unit1)
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

    IEnumerator CoCountDown(float countdown)
    {
        time = countdown;

        while(time > 0f)
        {
            UpdateUI();
            yield return new WaitForSeconds(1f);
            time -= 1f;
        }
    }

}
