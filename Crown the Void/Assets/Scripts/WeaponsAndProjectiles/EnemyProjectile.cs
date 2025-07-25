using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private EnemyBehavior enemyBehavior;
    [SerializeField] private bool destroyProjectileOnHit;
    [SerializeField] private AudioClip hitPlayerSound;
    [SerializeField] private AudioClip hitOtherSound;

    private bool canHitPlayer = true;
    private bool hitObject = false;

    private KnightBehavior kb;
    private PlayerController pc;
    private AudioSource m_AudioSource;

    private void Start()
    {
        kb = GameObject.FindWithTag("Player").GetComponent<KnightBehavior>();
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        m_AudioSource = GameObject.FindWithTag("Player").GetComponent<AudioSource>();
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
        if (collision.gameObject.tag == "Player")
        {
            if (canHitPlayer)   // Hit player
            {
                pc.TakeDamage(enemyBehavior.dmgPerHit);
                canHitPlayer = false;
                m_AudioSource.PlayOneShot(hitPlayerSound);
            }
            else if (!hitObject)    // Hit shield
            {
                m_AudioSource.PlayOneShot(hitOtherSound);
                hitObject = true;
            }
        }

        if (destroyProjectileOnHit)
        {
            Destroy(gameObject);
        }
    }
}
