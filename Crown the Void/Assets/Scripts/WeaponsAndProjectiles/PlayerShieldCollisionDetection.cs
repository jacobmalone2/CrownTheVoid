using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldCollisionDetection : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && playerController.IsBashing)
        {
            other.gameObject.GetComponent<EnemyBehavior>().Stun();
        }
    }
}
