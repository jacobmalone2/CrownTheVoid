using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldCollisionDetection : MonoBehaviour
{
    [SerializeField] private KnightBehavior kb;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && kb.IsBashing)
        {
            other.gameObject.GetComponent<EnemyBehavior>().Stun();
        }
    }
}
