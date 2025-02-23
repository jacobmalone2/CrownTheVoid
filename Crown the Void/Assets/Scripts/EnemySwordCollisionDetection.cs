using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordCollisionDetection : MonoBehaviour
{
    [SerializeField] private EnemyBehavior enemyBehavior;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && enemyBehavior.AlreadyAttacked && enemyBehavior.CanDealDamage)
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(2);
            enemyBehavior.HitPlayer();
        }
    }
}
