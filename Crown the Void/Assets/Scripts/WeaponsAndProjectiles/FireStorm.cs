using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStorm : MonoBehaviour
{
    private const float BURN_INTERVAL = 0.25f;

    [SerializeField] private int m_burnDamage = 1;
    [SerializeField] private float m_fireStormRadius = 5f;
    [SerializeField] private LayerMask m_enemyMask;
    [SerializeField] private int m_numFound;

    private Collider[] objectsInRange = new Collider[20];

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(BurnEnemies), BURN_INTERVAL);
    }

    private void BurnEnemies()
    {
        m_numFound = Physics.OverlapSphereNonAlloc(transform.position, 
            m_fireStormRadius, objectsInRange, m_enemyMask);

        for (int i = 0; i < m_numFound; i++)
        {
            objectsInRange[i].GetComponent<EnemyBehavior>().TakeDamage(m_burnDamage);
        }

        Invoke(nameof(BurnEnemies), BURN_INTERVAL);
    }
}
