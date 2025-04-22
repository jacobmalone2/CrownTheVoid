using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerProjectile : MonoBehaviour
{
    private const float PROJECTILE_LIFETIME = 8f;

    [SerializeField] private AudioClip hitTargetSound;

    private PlayerController pc;
    private Rigidbody rb;

    private bool m_targetHit;

    // Start is called before the first frame update
    void Start()
    {
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        Invoke(nameof(DestroyProjectile), PROJECTILE_LIFETIME);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only hit target if this is first collision
        if (m_targetHit || collision.gameObject.CompareTag("Player"))
            return;
        else
            m_targetHit = true;

        // Make projectile stick to surface and move with target
        rb.isKinematic = true;
        transform.SetParent(collision.transform);

        // Deal damage to enemies
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyBehavior>().TakeDamage(pc.AttackDamage);
            collision.gameObject.GetComponent<AudioSource>().PlayOneShot(hitTargetSound);   // Play arrow impact sound effect
            DestroyProjectile();
        }
        else if (collision.gameObject.CompareTag("Boss"))   // Deal damage to boss
        {
            collision.gameObject.GetComponent<BossBehavior>().TakeDamage(pc.AttackDamage);
            collision.gameObject.GetComponent<AudioSource>().PlayOneShot(hitTargetSound);   // Play arrow impact sound effect
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
