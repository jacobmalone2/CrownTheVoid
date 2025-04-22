using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossBehavior : MonoBehaviour
{
    private const float STUN_DURATION = 3f;

    [Header("Attack Settings")]
    [SerializeField] public int dmgPerHit = 1;
    [SerializeField] public int maxHealth = 500;
    [SerializeField] public int health = 10;
    //[SerializeField] FloatingHealthBar healthBar;
    [SerializeField] private int chargeSpeed = 2;
    [SerializeField] private int spinSpeed = 2;
    [SerializeField] private int runSpeed = 2;
    [SerializeField] private ParticleSystem deathVFX;
    [SerializeField] private GameObject battleAxe;
    [SerializeField] private GameObject SpinningAxePrefab;
    [SerializeField] private Transform ShootPoint;
    [SerializeField] private BossSwordCollisionDetection sword;
    [SerializeField] private FloatingHealthBar healthBar;

    //Phases
    private bool isPhaseOne = true;
    private bool isPhaseTwo = false;
    private bool isPhaseThree = false;
    

    private bool isAttacking = false;
    private bool isCharging = false;
    private bool isSpinning = false;
    private bool isRunningAway = false;
    private bool canDealDamage = true;
    private bool canTakeDamage = true;

    [Header("Game Settings")]
    [SerializeField] private float startDelay = 7.3f;
    [NonSerialized] public bool isAlive = false;
    private bool hasThrown = false;
    GameObject axeOfDeath;

    //Assigned in Start
    [NonSerialized] public GameObject player;
    [NonSerialized] public NavMeshAgent agent;
    [NonSerialized] public Animator enemyAnimator;

    private Vector3 walkPoint;
    [NonSerialized] private bool walkPointSet;
    [SerializeField] private float walkPointRange;
    [SerializeField] private LayerMask whatIsGround;

    public bool CanDealDamage { get => canDealDamage; set => canDealDamage = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public bool IsAlive { get => isAlive; }

    private Collider standingCollider;
    private Collider fallenCollider;
    PlayerController cs;

    // Start is called before the first frame update
    void Start()
    { 
        health = maxHealth;
        player = GameObject.FindWithTag("Player");
        cs = player.GetComponent<PlayerController>();

        agent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        standingCollider = GetComponent<CapsuleCollider>();
        fallenCollider = GetComponent<BoxCollider>();

        StartCoroutine(StartGame(startDelay));

        // playerCharacter = GameObject.FindWithTag("Player");
        // cs = playerCharacter.GetComponent<PlayerController>();

        //TODO Get Health Bar
        healthBar.UpdateHealthBar(health, maxHealth);
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
            if (isPhaseOne)
            {
                //Rushes the Player
                //If Player Dodges he keeps running for 1 more second and falls down
                //Stays down for 2 seconds
                //Gets up and does it again

                ChargePlayer();

                //Phase Change after he goes below 105 health
                //Animation plays between phases
            }
            else if (isPhaseTwo)
            {
                //Boss spins and follows the player
                //After a time frame he falls over
                //Stays down for 2 seconds
                //Gets up and does it again

                Spinney();

                //Phase change after he goes below 35 health
                //Animation plays between phases
            }
            else if (isPhaseThree)
            {
                //Boss throws his axe at player
                //He only does it once
                //After player dodges he can the idle boss for the win
                ThrowAxe();
            }

            //Check for Death
            if (cs.playerHealth <= 0)
            {
                isAlive = false;
                ResetAnimator();
                enemyAnimator.SetBool("isPlayerDead", true);
            }
        }
    }

    //Phase One
    private void ChargePlayer()
    {
        //Charge only happens once that he doesn't change direction
        if (!isCharging)
        {
            CanDealDamage = true;
            ResetAnimator();
            isCharging = true;
            // Get the target position to look at
            Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);

            // Calculate the "LookAt" rotation
            Quaternion lookRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);

            // Extract only the Y rotation 
            float yRotation = lookRotation.eulerAngles.y;

            // Apply only the Y rotation to the object
            transform.rotation = Quaternion.Euler(0, yRotation, 0);

            enemyAnimator.SetBool("isCharging", true);
            agent.SetDestination(targetPosition);
            agent.speed = chargeSpeed;
            agent.isStopped = false;
            fallenCollider.enabled = false;
            standingCollider.enabled = true;
        }
        else if (agent.transform.position == agent.pathEndPosition && !enemyAnimator.GetBool("isStunned") && isCharging)
        {
            ResetAnimator();
            enemyAnimator.SetBool("isStunned", true);
            fallenCollider.enabled = true;
            standingCollider.enabled = false;
            agent.isStopped = true;
            StartCoroutine(BossStunned(STUN_DURATION));
        }
    }

    IEnumerator BossStunned(float STUN_DURATION)
    {
        CanDealDamage = false;
        yield return new WaitForSeconds(STUN_DURATION);
        isCharging = false;
        isSpinning = false;
    }

    //Phase Two
    private void Spinney()
    {
        if(!isSpinning)
        {
            ResetAnimator();
            isSpinning = true;
            agent.isStopped = false;
            enemyAnimator.SetBool("isSpinning", true);
            fallenCollider.enabled = false;
            standingCollider.enabled = true;
            CanDealDamage = true;
            StartCoroutine(BossSpinney());
        }

        if(!enemyAnimator.GetBool("isStunned"))
        {
            
            agent.SetDestination(player.transform.position);
            agent.speed = spinSpeed;
        }
    }

    IEnumerator BossSpinney()
    {
        yield return new WaitForSeconds(6);
        agent.isStopped = true;
        ResetAnimator();
        enemyAnimator.SetBool("isStunned", true);
        fallenCollider.enabled = true;
        standingCollider.enabled = false;
        StartCoroutine(BossStunned(STUN_DURATION));
    }

    //Phase Three
    private void ThrowAxe()
    {
        if (!hasThrown)
        {
            fallenCollider.enabled = false;
            standingCollider.enabled = true;
            agent.isStopped = true;
            enemyAnimator.SetBool("isThrowing", true);

            // Get the target position to look at
            Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);

            // Calculate the "LookAt" rotation
            Quaternion lookRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);

            // Extract only the Y rotation 
            float yRotation = lookRotation.eulerAngles.y;

            // Apply only the Y rotation to the object
            transform.rotation = Quaternion.Euler(0, yRotation, 0);

            hasThrown = true;
            
            StartCoroutine(ShootDelay(1.5f));
        }
        else
        {
            if (isRunningAway)
            {
                canDealDamage = false;
                ResetAnimator();
                enemyAnimator.SetBool("isCharging", true);
                agent.speed = runSpeed;
                RunAway();
            }
        }
    }

    public IEnumerator ShootDelay(float shootDelay)
    {
        yield return new WaitForSeconds(shootDelay);

        battleAxe.SetActive(false);
        axeOfDeath = Instantiate(SpinningAxePrefab, ShootPoint.transform.position, SpinningAxePrefab.transform.rotation);

        ResetAnimator();
        agent.isStopped = false;
        isRunningAway = true;
    }

    private void RunAway()
    {
        isAttacking = false;    // Makes sure player doesn't take damage when not being actively attacked

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
        if (canTakeDamage && isAlive)
        {
            health -= damage;
            healthBar.UpdateHealthBar(health, maxHealth);
        }

        if(health <= 300 && isPhaseOne)
        {
            ResetAnimator();
            isPhaseOne = false;
            StartCoroutine(PhaseChange(2));
        }
        else if(health <= 100 && isPhaseTwo)
        {
            ResetAnimator();
            isPhaseTwo = false;
            StartCoroutine(PhaseChange(3));
        }
        else if (health <= 0 && isPhaseThree)
        {
            isAlive = false;
            agent.isStopped = true;
            axeOfDeath.SetActive(false);
            var axeDeathVFX = Instantiate(deathVFX, axeOfDeath.transform.position, Quaternion.identity);
            var deathEffect = Instantiate(deathVFX, transform.position, Quaternion.identity);
            deathEffect.transform.parent = gameObject.transform;
            deathVFX.Play();

            ResetAnimator();
            enemyAnimator.SetBool("isDying", true);

            Invoke(nameof(DestroyEnemy), 5f);
        }
    }

    IEnumerator PhaseChange(int phase)
    {
        CanDealDamage = false;
        ResetAnimator();
        fallenCollider.enabled = false;
        standingCollider.enabled = true;
        enemyAnimator.SetTrigger("PhaseChange");
        yield return new WaitForSeconds(3f);
        if(phase == 2)
        {
            isPhaseTwo = true;
        }
        else if (phase == 3)
        {
            isPhaseThree = true;
        }
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

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    
}
