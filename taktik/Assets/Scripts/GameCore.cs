using UnityEngine;
using System.Collections;

// * draw no kill
// * 2 lives
// * scores
// * 
public class GameCore : UKUnitySingletonManuallyCreated<GameCore> {
    public float RoundTime;
    
    public int RoundIndex;

    public Player Player0;
    public Player Player1;

    private float time;

    void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        UIGlue.Instance.Player0Score.text = string.Format("{0:0.} points", Player0.Score);
        UIGlue.Instance.Player1Score.text = string.Format("{0:0.} points", Player1.Score);

        UIGlue.Instance.Player0Time.text = "";// string.Format("{0:0.} sec", time);
        UIGlue.Instance.Player1Time.text = "";// string.Format("{0:0.} sec", time);
    }

    public static bool Ignores(Unit.UnitType a, Unit.UnitType b)
    {
        return a == b;
    }

    public static bool Beats(Unit.UnitType a, Unit.UnitType b)
    {
        return (a == Unit.UnitType.PAPER && b == Unit.UnitType.ROCK) ||
            (a == Unit.UnitType.ROCK && b == Unit.UnitType.SCISSOR) ||
            (a == Unit.UnitType.SCISSOR && b == Unit.UnitType.PAPER);
    }

    // returns which unit dies, true -> dead or damage
    public static UKTuple<bool,bool> CalculateFightDamage(Unit unit0, Unit unit1)
    {
        // empty
        if (unit0 == null && unit1 == null) return new UKTuple<bool,bool>(false, false);
        if (unit0 == null && unit1 != null) return new UKTuple<bool, bool>(false, false);
        if (unit0 != null && unit1 == null) return new UKTuple<bool, bool>(false, false);
        // totally complex mechanic
        var t0 = unit0.Type;
        var t1 = unit1.Type;
        if (t0 == t1) return new UKTuple<bool, bool>(false, false);
        if (Beats(t0, t1)) return new UKTuple<bool, bool>(false, true);
        if (Beats(t1, t0)) return new UKTuple<bool, bool>(true, false);
        return new UKTuple<bool, bool>(false, false);
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


    public void Score(int playerId, int score)
    {
        if (playerId == 0) Player0.Score += score;
        else if (playerId == 1) Player1.Score += score;
        UpdateUI();
    }
}
