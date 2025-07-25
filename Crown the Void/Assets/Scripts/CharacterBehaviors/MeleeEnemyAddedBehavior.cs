using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAddedBehavior : MonoBehaviour
{
    private const float CAN_DAMAGE_AFTER_HIT = 0.6f;
    [SerializeField] private EnemySwordCollisionDetection sword;

    [SerializeField] private float timeBetweenAttacks = 1f;

    [SerializeField] private int runSpeed = 4;

    private bool alreadyAttacked;

    private EnemyBehavior eB;
    private Animator enemyAnimator;
    private NavMeshAgent agent;
    private PlayerController pc;

    // Start is called before the first frame update
    void Start()
    {
        eB = GetComponent<EnemyBehavior>();
        enemyAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(eB.isAlive)
        {
            if (eB.playerInSightRange && !eB.playerInAttackRange) ChasePlayer();
            if (eB.playerInSightRange && eB.playerInAttackRange) MeleePlayer();
        }
    }

    private void ChasePlayer()
    {
        eB.IsAttacking = false;    // Makes sure player doesn't take damage when not being actively attacked

        if (!enemyAnimator.GetBool("isRunning"))
        {
            agent.speed = runSpeed;
            eB.ResetAnimator();
            enemyAnimator.SetBool("isRunning", true);
        }

        agent.SetDestination(eB.player.position);
    }

    private void MeleePlayer()
    {
        agent.SetDestination(transform.position);

        // Get the target position to look at
        Vector3 targetPosition = new Vector3(eB.player.transform.position.x, eB.player.transform.position.y, eB.player.transform.position.z);

        // Calculate the "LookAt" rotation
        Quaternion lookRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);

        // Extract only the Y rotation 
        float yRotation = lookRotation.eulerAngles.y;

        // Apply only the Y rotation to the object
        transform.rotation = Quaternion.Euler(0, yRotation, 0); 

        if (!alreadyAttacked)
        {
            enemyAnimator.SetInteger("attackIndex", UnityEngine.Random.Range(0, 4));
            //Attack Code

            eB.IsAttacking = true;

            if (!enemyAnimator.GetBool("isAttacking"))
            {
                eB.ResetAnimator();
                enemyAnimator.SetBool("isAttacking", true);
            }

            alreadyAttacked = true;

            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void CheckDealDamage()
    {
        sword.CheckDealDamage();
    }

    public void PlaySwingSoundEffect()
    {
        sword.PlaySwingSoundEffect();
    }
}
