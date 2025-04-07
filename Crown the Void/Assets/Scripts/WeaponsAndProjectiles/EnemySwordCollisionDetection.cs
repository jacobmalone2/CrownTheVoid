using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySwordCollisionDetection : MonoBehaviour
{
    [SerializeField] private EnemyBehavior enemyBehavior;
    [SerializeField] private MeleeEnemyAddedBehavior meleeBehavior;

    private bool canHitPlayer = true;
    private bool isTouching = false;

    private PlayerController pc;
    private KnightBehavior kb;

    private void Start()
    {
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        kb = GameObject.FindWithTag("Player").GetComponent<KnightBehavior>();
    }

    public void CheckDealDamage()
    {
        if(isTouching && canHitPlayer)
        {
            pc.TakeDamage(enemyBehavior.dmgPerHit);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            isTouching = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Shield"))
        {
            if (kb.IsBlocking)
                canHitPlayer = false;
            else if (!kb.IsBlocking)
                canHitPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            isTouching = false;

        if (other.gameObject.CompareTag("Shield"))
            canHitPlayer = true;
    }
}
