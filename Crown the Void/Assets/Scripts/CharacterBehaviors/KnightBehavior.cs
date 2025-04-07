using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KnightBehavior : MonoBehaviour
{
    private const float ATTACK_DURATION = 0.7f;
    private const float BASH_DURATION = 0.4f;

    private bool m_isAttacking = false;
    private bool m_isBashing = false;
    private bool m_isBlocking = false;

    private PlayerController pc;
    private Animator m_Animator;

    public bool IsAttacking { get => m_isAttacking; }
    public bool IsBlocking { get => m_isBlocking; }
    public bool IsBashing { get => m_isBashing; }



    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.IsAlive)
        {
            // Check for knight specific action each frame

            CheckForKnightAction();
        }
    }

    private void CheckForKnightAction()
    {
        // Sword swing attack
        if (Input.GetMouseButtonDown((int)MouseButton.Left) && !pc.TakingAction)
        {
            Attack();
        }
        // Start Block
        if (Input.GetMouseButtonDown((int)MouseButton.Right) && !pc.TakingAction)
        {
            StartBlock();
        }
        // Shield Bash
        if (Input.GetMouseButtonDown((int)MouseButton.Left) && m_isBlocking)
        {
            ShieldBash();
        }
        // End Block
        if (Input.GetMouseButtonUp((int)MouseButton.Right) && m_isBlocking)
        {
            EndBlock();
        }
    }

    //-------------------------------------------------------------------------
    //*************************************************************************
    //                             ACTION METHODS
    //*************************************************************************
    //-------------------------------------------------------------------------


    //-------------------------------------------------------------------------
    // Triggers the attack animation, set the attack and ability flags, and
    // initiate attack cooldown
    //-------------------------------------------------------------------------
    private void Attack()
    {
        m_Animator.SetTrigger("Attack");
        m_isAttacking = true;
        pc.TakingAction = true;
        StartCoroutine(AttackCooldown());
    }

    // Cooldown for sword swing attack
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(ATTACK_DURATION);
        m_isAttacking = false;
        if (!m_isBlocking) pc.TakingAction = false;
    }

    //---------------------------------------------------------------------------
    // Triggers the raise shield animation and sets flags for starting to block
    //---------------------------------------------------------------------------
    private void StartBlock()
    {
        m_Animator.SetBool("IsBlocking", true);
        pc.TakingAction = true;
        m_isBlocking = true;
    }

    //------------------------------------------------------------------------------
    // Triggers the bash animation and sets the shield bash flag for bash duration
    //------------------------------------------------------------------------------
    private void ShieldBash()
    {
        m_Animator.SetTrigger("Bash");
        m_isBashing = true;
        Invoke(nameof(ShieldBashReset), BASH_DURATION);
    }

    private void ShieldBashReset()
    {
        m_isBashing = false;
    }

    //----------------------------------------------------------------
    // Ends the block animation and clears the flags for blocking
    //----------------------------------------------------------------
    private void EndBlock()
    {
        m_Animator.SetBool("IsBlocking", false);
        pc.TakingAction = false;
        m_isBlocking = false;
    }
}
