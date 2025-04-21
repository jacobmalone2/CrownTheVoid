using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordCollisionDetection : MonoBehaviour
{
    private const float ATTACK_ANIMATION_DURATION = 1.067f;
    private const float ANIMATION_SPEED_MULTIPLIER = 1.5f;
    private const int KNIGHT_ANIMATION_LAYER = 4;

    private PlayerController pc;
    private KnightBehavior kb;
    private Animator animator;
    private AudioSource audioSource;

    [SerializeField] private AudioClip impactSound;

    private void Start()
    {
        pc = GetComponentInParent<PlayerController>();
        kb = GetComponentInParent<KnightBehavior>();
        animator = GetComponentInParent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && kb.IsAttacking)
        {
            EnemyBehavior eb = other.gameObject.GetComponent<EnemyBehavior>();
            eb.TakeDamage(pc.AttackDamage);

            // Play impact sound
            if (!audioSource.isPlaying) audioSource.PlayOneShot(impactSound);

            // Get time remaining in attack animation and stop enemy from taking damage for remainder of attack
            float timeRemaining = ATTACK_ANIMATION_DURATION / ANIMATION_SPEED_MULTIPLIER -
                animator.GetCurrentAnimatorStateInfo(KNIGHT_ANIMATION_LAYER).normalizedTime / ANIMATION_SPEED_MULTIPLIER;

            eb.StopDamageForTime(timeRemaining);
        }

        if (other.gameObject.CompareTag("Boss") && kb.IsAttacking)
        {
            BossBehavior bb = other.gameObject.GetComponent<BossBehavior>();
            bb.TakeDamage(pc.AttackDamage);

            // Play impact sound
            if (!audioSource.isPlaying) audioSource.PlayOneShot(impactSound);

            // Get time remaining in attack animation and stop enemy from taking damage for remainder of attack
            float timeRemaining = ATTACK_ANIMATION_DURATION / ANIMATION_SPEED_MULTIPLIER -
                animator.GetCurrentAnimatorStateInfo(KNIGHT_ANIMATION_LAYER).normalizedTime / ANIMATION_SPEED_MULTIPLIER;

            bb.StopDamageForTime(timeRemaining);
        }
    }
}
