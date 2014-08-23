using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {
    public enum UnitType {
        SCISSOR = 0,
        ROCK = 1,
        PAPER = 2,
    }

    public int PlayerId;

    public UnitType Type;

    void OnTriggerEnter(Collider other)
    {
        Unit otherUnit = other.gameObject.GetComponent<Unit>();
        if (otherUnit != null)
        {
            if (otherUnit.PlayerId != PlayerId)
            {
                int winner = GameCore.CalculateWinner(this, otherUnit);
                // draw or other unit wins -> die
                if (winner == -1 || winner == 1)
                {
                    GameObject.Destroy(gameObject);
                }
            }
        }
    }
}
