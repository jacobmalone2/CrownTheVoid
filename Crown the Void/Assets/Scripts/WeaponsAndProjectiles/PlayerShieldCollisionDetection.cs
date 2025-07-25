using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldCollisionDetection : MonoBehaviour
{
    [SerializeField] private KnightBehavior kb;
    [SerializeField] private AudioClip shieldBashSound;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && kb.IsBashing)
        {
            if (!other.gameObject.GetComponent<EnemyBehavior>().IsStunned)
                GetComponent<AudioSource>().PlayOneShot(shieldBashSound);
            other.gameObject.GetComponent<EnemyBehavior>().Stun();
        }
    }
}
