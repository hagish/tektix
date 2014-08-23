﻿using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {
    public enum UnitType {
        SCISSOR = 0,
        ROCK = 1,
        PAPER = 2,
    }

    public int PlayerId;

    public UnitType Type;

    public int Lives = 2;

    void OnTriggerEnter(Collider other)
    {
        Unit otherUnit = other.gameObject.GetComponent<Unit>();
        if (otherUnit != null)
        {
            if (otherUnit.PlayerId != PlayerId)
            {
                var result = GameCore.CalculateFightDamage(this, otherUnit);
                // if i lost kill me
                if (result.a) Kill();
                // but if the other one loses i get damage
                else if (result.b) GetDamage();
            }
        }

        ScoreArea scoreArea = other.gameObject.GetComponent<ScoreArea>();
        if (scoreArea != null && scoreArea.PlayerId != PlayerId)
        {
            GameCore.Instance.Score(PlayerId, 1);
            Kill();
        }
    }

    public void Kill()
    {
        GameObject.Destroy(gameObject);
    }

    public void GetDamage()
    {
        Lives--;

        if (Lives == 0)
        {
            Kill();
        }
    }
}
