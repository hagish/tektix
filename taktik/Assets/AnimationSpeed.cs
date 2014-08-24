using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class AnimationSpeed : MonoBehaviour {
    public float AnimSpeed;
    public float GamePlaySpeed;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

	// Update is called once per frame
	void Update () 
    {
        if (AnimSpeed > 0f)
        {
            var factor = GamePlaySpeed / AnimSpeed;
            animator.speed = factor;
        }
        else
        {
            animator.speed = 1f;
        }
	}
}
