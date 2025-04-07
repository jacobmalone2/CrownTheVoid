using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private EnemyBehavior enemyBehavior;
    [SerializeField] private bool destroyProjectileOnHit;

    private bool canHitPlayer = true;

    private KnightBehavior kb;
    private PlayerController pc;

    private void Start()
    {
        kb = GameObject.FindWithTag("Player").GetComponent<KnightBehavior>();
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Shield") && kb.IsBlocking)
        {
            canHitPlayer = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && canHitPlayer)
        {
            pc.TakeDamage(enemyBehavior.dmgPerHit);
            canHitPlayer = false;
        }
        if (destroyProjectileOnHit)
        {
            Destroy(gameObject);
        }
    }
}
