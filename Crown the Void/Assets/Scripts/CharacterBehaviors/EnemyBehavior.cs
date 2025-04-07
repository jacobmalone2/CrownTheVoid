using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{
    private const float CAN_DAMAGE_AFTER_SHIELD_HIT = 1f;
    private const float TAKE_DAMAGE_COOLDOWN = 1.0f;
    private const float STUN_DURATION = 1.72f;

    private bool isStunned = false;

    [Header("AI Fields")]
    [SerializeField] private float walkPointRange;
    [SerializeField] private float sightRange, attackRange;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    [NonSerialized] public bool playerInSightRange, playerInAttackRange;
    private Vector3 walkPoint;
    [NonSerialized] private bool walkPointSet;

    [Header("Attack Settings")]
    [SerializeField] public int dmgPerHit = 10;

    [SerializeField] public int maxHealth = 10;
    [NonSerialized] public int health = 10;
    [SerializeField] FloatingHealthBar healthBar;
    [SerializeField] private int walkSpeed = 2;
    [SerializeField] private ParticleSystem deathVFX;
    
    private bool isAttacking = false;
    private bool canDealDamage = true;
    private bool canTakeDamage = true;

    [Header("Game Settings")]
    [SerializeField] private float startDelay = 7.3f;

    [NonSerialized] public bool isAlive = false;

    //Assigned in Start
    [NonSerialized] public Transform player;
    [NonSerialized] public NavMeshAgent agent;
    private Animator enemyAnimator;

    public bool CanDealDamage { get => canDealDamage; set => canDealDamage = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public bool IsAlive { get => isAlive; }

    private GameObject playerCharacter;
    PlayerController cs;

    // Start is called before the first frame update
    void Start()
    { 
        health = maxHealth;
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();

        StartCoroutine(StartGame(startDelay));

        playerCharacter = GameObject.FindWithTag("Player");
        cs = playerCharacter.GetComponent<PlayerController>();

        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.UpdateHealthBar(health, maxHealth);
    }

    IEnumerator StartGame(float startDelay)
    {
        yield return new WaitForSeconds(startDelay);
        Debug.Log("isAlive True");
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

            if (cs.playerHealth <= 0)
            {
                isAlive = false;
                ResetAnimator();
                enemyAnimator.SetBool("isPlayerDead", true);
            }
        }

    }

    private void Patroling()
    {
        isAttacking = false;    // Makes sure player doesn't take damage when not being actively attacked

        if (!enemyAnimator.GetBool("isWalking"))
        {
            agent.speed = walkSpeed;
            
            ResetAnimator();
            enemyAnimator.SetBool("isWalking", true);
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
            StopCoroutine(WalkPointTimeout());
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        StartCoroutine(WalkPointTimeout());

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    IEnumerator WalkPointTimeout()
    {
        yield return new WaitForSeconds(4f);
        walkPointSet = false;
    }

    public void TakeDamage(int damage)
    {
        if (canTakeDamage)
        {
            health -= damage;
            healthBar.UpdateHealthBar(health, maxHealth);

            if (health <= 0)
            {
                isAlive = false;

                var deathEffect = Instantiate(deathVFX, transform.position, Quaternion.identity);
                deathEffect.transform.parent = gameObject.transform;
                deathVFX.Play();

                ResetAnimator();
                enemyAnimator.SetBool("isDying", true);

                Invoke(nameof(DestroyEnemy), 2.5f);
            }
        }
    }

    // Spawns a timer that prevents the enemy from taking damage
    public void StopDamageForTime(float time)
    {
        canTakeDamage = false;
        Invoke(nameof(TakeDamageTimer), time);
    }

    private void TakeDamageTimer()
    {
        canTakeDamage = true;
    }

    public void Stun()
    {
        if (!isStunned)
        {
            enemyAnimator.SetTrigger("stun");
            CanDealDamage = false;
            isStunned = true;

            Invoke(nameof(EndStun), STUN_DURATION);
        }
    }

    private void EndStun()
    {
        CanDealDamage = true;
        isStunned = false;
    }

    public void ResetAnimator()
    {
        foreach(AnimatorControllerParameter parameter in enemyAnimator.parameters) {
            if(parameter.name != "attackIndex")
            {
                enemyAnimator.SetBool(parameter.name, false);   
            }          
        }
    } 

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    //Draws the Spheres so we can visually see the ranges on the enemy
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
