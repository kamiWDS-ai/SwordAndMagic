using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior_simple : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (SlotManager.Instance != null)
            SlotManager.Instance.OnTripleMatched += Attack;
    }

    private void OnDisable()
    {
        if (SlotManager.Instance != null)
            SlotManager.Instance.OnTripleMatched -= Attack;
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
    }
}
