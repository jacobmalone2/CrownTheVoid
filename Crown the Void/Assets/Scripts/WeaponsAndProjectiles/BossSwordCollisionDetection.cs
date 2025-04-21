using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSwordCollisionDetection : MonoBehaviour
{
    [SerializeField] private BossBehavior bossBehavior;
    //[SerializeField] private AudioClip impactSound;
    //[SerializeField] private AudioClip[] swingSounds;

    private bool canHitPlayer = true;
    private bool isTouching = false;

    private PlayerController pc;
    private KnightBehavior kb;
    private AudioSource m_AudioSource;

    private void Start()
    {
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        kb = GameObject.FindWithTag("Player").GetComponent<KnightBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        pc.TakeDamage(bossBehavior.dmgPerHit);
    }
}
