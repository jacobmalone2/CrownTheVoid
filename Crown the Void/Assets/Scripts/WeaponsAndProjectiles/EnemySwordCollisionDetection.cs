using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySwordCollisionDetection : MonoBehaviour
{
    [SerializeField] private EnemyBehavior enemyBehavior;
    [SerializeField] private MeleeEnemyAddedBehavior meleeBehavior;
    [SerializeField] private AudioClip blockSound;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private AudioClip[] swingSounds;

    private bool canHitPlayer = true;
    private bool isTouching = false;

    private PlayerController pc;
    private KnightBehavior kb;
    private AudioSource m_AudioSource;

    private void Start()
    {
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        kb = GameObject.FindWithTag("Player").GetComponent<KnightBehavior>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void CheckDealDamage()
    {
        if(isTouching)
        {
            if (canHitPlayer)
            {
                pc.TakeDamage(enemyBehavior.dmgPerHit);
                m_AudioSource.PlayOneShot(impactSound);
            }
            else
            {
                m_AudioSource.PlayOneShot(blockSound); // Play block sound effect
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            isTouching = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Shield"))
        {
            if (kb.IsBlocking)
                canHitPlayer = false;
            else if (!kb.IsBlocking)
                canHitPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            isTouching = false;

        if (other.gameObject.CompareTag("Shield"))
            canHitPlayer = true;
    }

    public void PlaySwingSoundEffect()
    {
        m_AudioSource.PlayOneShot(swingSounds[Random.Range(0, swingSounds.Length)]);
    }
}
