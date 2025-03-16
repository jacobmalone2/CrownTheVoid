using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{
    private const float CAN_DAMAGE_COOLDOWN = 0.6f;
    private const float TAKE_DAMAGE_COOLDOWN = 1.0f;

    [Header("AI Fields")]
    [SerializeField]
    private float walkPointRange;
    [SerializeField]
    private float sightRange, attackRange;
    [SerializeField]
    private LayerMask whatIsGround, whatIsPlayer;
    private bool playerInSightRange, playerInAttackRange;
    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Attack Settings")]

    [SerializeField]
    private float timeBetweenAttacks = 1f;
    [SerializeField] public int dmgPerHit = 10;

    [SerializeField]
    public int health = 10;
    [SerializeField]
    FloatingHealthBar healthBar;
    [SerializeField]
    private int walkSpeed = 2;
    [SerializeField]
    private int runSpeed = 4;
    [SerializeField]
    private ParticleSystem deathVFX;
    private bool alreadyAttacked;
    private bool canDealDamage = true;
    private bool canTakeDamage = true;

    [Header("Game Settings")]
    [SerializeField]
    private float startDelay = 7.3f;
    bool isAlive = false;

    //Assigned in Start
    private Transform player;
    private NavMeshAgent agent;
    private Animator minionAnimator;

    public bool AlreadyAttacked { get => alreadyAttacked; }
    public bool CanDealDamage { get => canDealDamage; }

    private GameObject playerCharacter;
    PlayerController cs;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
        minionAnimator = GetComponent<Animator>();
        StartCoroutine(StartGame(startDelay));
        playerCharacter = GameObject.FindWithTag("Player");
        cs = playerCharacter.GetComponent<PlayerController>();
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.UpdateHealthBar(health, 10);


    }

    IEnumerator StartGame(float startDelay)
    {
        yield return new WaitForSeconds(startDelay);
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isAlive)
        {
            //Draws a Sphere from around the Enemy and checks if the Player is within it 
            //It does this by using the whatIsPlayer Layer
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            //Tests where the player is in relation to itself and responds accordingly
            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInSightRange && playerInAttackRange) AttackPlayer();

            if (cs.playerHealth <= 0)
            {
                isAlive = false;
                minionAnimator.SetBool("isRunning", false);
                minionAnimator.SetBool("isWalking", false);
                minionAnimator.SetBool("isAttacking", false);
                minionAnimator.SetBool("isPlayerDead", true);
            }
        }

    }

    private void Patroling()
    {
        if(!minionAnimator.GetBool("isWalking"))
        {
            Debug.Log("Walking");
            agent.speed = walkSpeed;
            minionAnimator.SetBool("isRunning", false);
            minionAnimator.SetBool("isAttacking", false);
            minionAnimator.SetBool("isWalking", true);
        }


        if (!walkPointSet) SearchWalkPoint();

        if(walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        if(!minionAnimator.GetBool("isRunning"))
        {
            Debug.Log("Running");
            agent.speed = runSpeed;
            minionAnimator.SetBool("isWalking", false);
            minionAnimator.SetBool("isAttacking", false);
            minionAnimator.SetBool("isRunning", true);
        }

        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        // Get the target position to look at
        Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);

        // Calculate the "LookAt" rotation
        Quaternion lookRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);

        // Extract only the Y rotation 
        float yRotation = lookRotation.eulerAngles.y;

        // Apply only the Y rotation to the object
        transform.rotation = Quaternion.Euler(0, yRotation, 0); 

        if (!alreadyAttacked)
        {
            minionAnimator.SetInteger("attackIndex", UnityEngine.Random.Range(0, 5));
            //Attack Code
            
            if(!minionAnimator.GetBool("isAttacking"))
            {
                Debug.Log("Attack");
                minionAnimator.SetBool("isRunning", false);
                minionAnimator.SetBool("isWalking", false);
                minionAnimator.SetBool("isAttacking", true);
            }

            alreadyAttacked = true;

            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        if (canTakeDamage)
        {
            health -= damage;
            healthBar.UpdateHealthBar(health, 10);
            canTakeDamage = false;
            Invoke(nameof(TakeDamageCooldown), TAKE_DAMAGE_COOLDOWN);
            Debug.Log("You got me");

            if (health <= 0)
            {
                isAlive = false;

                var deathEffect = Instantiate(deathVFX, transform.position, Quaternion.identity);
                deathEffect.transform.parent = gameObject.transform;
                deathVFX.Play();

                minionAnimator.SetBool("isRunning", false);
                minionAnimator.SetBool("isWalking", false);
                minionAnimator.SetBool("isAttacking", false);
                minionAnimator.SetBool("isDying", true);

                Invoke(nameof(DestroyEnemy), 2.5f);
            }
        }
    }

    private void TakeDamageCooldown()
    {
        canTakeDamage = true;
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    // When we deal damage to the player, spawn a timer before we can deal damage again
    public void HitPlayer()
    {
        canDealDamage = false;
        Invoke(nameof(CanDamageCooldown), CAN_DAMAGE_COOLDOWN);
    }

    private void CanDamageCooldown()
    {
        canDealDamage = true;
    }

    //Draws the Spheres so we can visually see the ranges on the enemy
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if(other.gameObject.tag == "Player")
    //     {
    //         isPlaying = false;
    //         minionAnimator.SetBool("isRunning", false);
    //         minionAnimator.SetBool("isWalking", false);
    //         minionAnimator.SetBool("isAttacking", false);
    //         minionAnimator.SetBool("isPlayerDead", true);
    //     }
    // }
}
