using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {
    public enum UnitType {
        SCISSOR = 0,    // red
        ROCK = 1,   // green
        PAPER = 2,  // yellow
    }

    public Transform RotationRoot;
    public bool IsRotated;
    public Vector3 LocalRotation;
    public float RotationSpeed;

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
                if (result.a) Kill(true);
                // but if the other one loses i get damage
                else if (result.b) GetDamage();
            }
        }

        ScoreArea scoreArea = other.gameObject.GetComponent<ScoreArea>();
        if (scoreArea != null && scoreArea.PlayerId != PlayerId)
        {
            AudioController.Instance.PlaySound("score");
            GameCore.Instance.Score(PlayerId, 1);
            Kill(false);
        }
    }

    public void Kill(bool playSound)
    {
        GameObject.Destroy(gameObject);
        if (playSound) AudioController.Instance.PlaySound("kill");
    }

    public void GetDamage()
    {
        Lives--;

        if (Lives == 0)
        {
            Kill(true);
        }
    }

    void Update()
    {
        var target = IsRotated ? LocalRotation : Vector3.zero;
        RotationRoot.localRotation = Quaternion.RotateTowards(RotationRoot.localRotation, Quaternion.Euler(target), Time.deltaTime * RotationSpeed);
    }
}
