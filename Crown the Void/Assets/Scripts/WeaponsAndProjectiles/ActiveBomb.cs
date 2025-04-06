using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveBomb : MonoBehaviour
{
    private const float EXPLODE_TIMER = 3f;

    [SerializeField] private float explosionRadius = 7f;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private int explosionDamage = 10;
    [SerializeField] private GameObject explosionEffect;

    private bool hasExploded = false;

    // Set a timer for bomb explosion
    private void Start()
    {
        Invoke(nameof(Explode), EXPLODE_TIMER);
    }

    public void Explode()
    {
        if (hasExploded) return;
        
        hasExploded = true;

        // Spawn explosion particle effect
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Get all objects in range of explosion
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, explosionRadius);

        // Loop through all objects and look for enemies
        for (int i = 0; i < objectsInRange.Length; i++)
        {
            if (objectsInRange[i].gameObject.CompareTag("Enemy"))
            {
                // Apply explosion damage
                objectsInRange[i].GetComponent<EnemyBehavior>().TakeDamage(explosionDamage);

                // Find explosion force direction vector
                Vector3 enemyPos = objectsInRange[i].transform.position;
                enemyPos.y = 0;

                Vector3 bombPos = transform.position;
                bombPos.y = 0;

                Vector3 forceDirection = (enemyPos - transform.position).normalized;

                // Apply explosion force
                objectsInRange[i].GetComponent<Rigidbody>().AddForce(forceDirection * explosionForce,
                    ForceMode.Impulse);
            }
        }

        // Destroy bomb after short delay
        Invoke(nameof(DestroyObject), 0.1f);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
