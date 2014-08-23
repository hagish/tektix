using UnityEngine;
using System.Collections;

public class AudioRandomSound : MonoBehaviour {
    public float MinWait;
    public float MaxWait;
    public string Sound;

	// Use this for initialization
	IEnumerator Start () {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MinWait, MaxWait));
            AudioController.Instance.PlaySound(Sound);
        }
	}
}
