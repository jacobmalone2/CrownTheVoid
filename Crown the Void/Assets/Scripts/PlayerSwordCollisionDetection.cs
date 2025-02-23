using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordCollisionDetection : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && playerController.IsAttacking)
        {
            other.gameObject.GetComponent<EnemyBehavior>().TakeDamage(5);
        }
    }
}
