using UnityEngine;
using System.Collections;

public class Blinking : MonoBehaviour 
{
    private bool isBlinking = false;
    public float TimeOn = 1f;
    public float TimeOff = 0.5f;

    IEnumerator Start()
    {
        while(true)
        {
            if (isBlinking) renderer.enabled = false;
            yield return new WaitForSeconds(TimeOff);
            renderer.enabled = true;
            yield return new WaitForSeconds(TimeOn);
        }
    }

    public void StartBlinking()
    {
        isBlinking = true;
    }

    public void StopBlinking()
    {
        isBlinking = false;
    }
}
