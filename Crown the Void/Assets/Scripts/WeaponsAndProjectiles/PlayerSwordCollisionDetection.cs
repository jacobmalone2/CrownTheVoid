using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordCollisionDetection : MonoBehaviour
{
    private const float ATTACK_ANIMATION_DURATION = 1.067f;
    private const float ANIMATION_SPEED_MULTIPLIER = 1.5f;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && playerController.IsAttacking)
        {
            EnemyBehavior eb = other.gameObject.GetComponent<EnemyBehavior>();
            eb.TakeDamage(playerController.AttackDamage);

            // Get time remaining in attack animation and stop enemy from taking damage for remainder of attack
            float timeRemaining = ATTACK_ANIMATION_DURATION / ANIMATION_SPEED_MULTIPLIER -
                animator.GetCurrentAnimatorStateInfo(1).normalizedTime / ANIMATION_SPEED_MULTIPLIER;

            eb.StopDamageForTime(timeRemaining);
        }
    }
}
