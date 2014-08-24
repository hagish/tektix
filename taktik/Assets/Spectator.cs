using UnityEngine;
using System.Collections;

public class Spectator : MonoBehaviour {
    private Animator animator;
    public float InitialDelay = 1f;
    public float CheerPercentage = 0.2f;
    public float IdleTimeout = 10f;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(CoIdle());

        UKMessenger.AddListener("Cheer", gameObject, Cheer);
    }

    IEnumerator CoIdle()
    {
        while(true)
        {
            yield return new WaitForSeconds(IdleTimeout * Random.value);
            animator.SetTrigger("Jump");
        }
    }

    public void Cheer()
    {
        if (Random.value < CheerPercentage)
        {
            StartCoroutine(CoJump());
        }
    }

	IEnumerator CoJump () 
    {
        yield return new WaitForSeconds(Random.value * InitialDelay);
        animator.SetTrigger("Jump");
	}
}
