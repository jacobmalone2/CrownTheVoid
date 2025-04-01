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

    private void Start()
    {
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
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
        {
            isTouching = true;
        }

        if (other.gameObject.CompareTag("Shield") && pc.IsBlocking)
        {
            canHitPlayer = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isTouching = false;

        if (other.gameObject.CompareTag("Shield"))
        {
            canHitPlayer = true;
        }
    }
}
