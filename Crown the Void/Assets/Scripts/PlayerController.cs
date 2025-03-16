using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private const float ATTACK_DURATION = 0.8f;
    private const float DODGE_DURATION = 0.2f;
    private const float DODGE_COOLDOWN = 1f;
    public float DODGE_CURRENT_COOLDOWN = 0f;
    private const float DODGE_SPEED_MULTIPLIER = 5f;

    private enum MoveAnim   // Enumeration type to define movement directions for animations
    {
        Idle,
        Forward,
        Backward,
        Left,
        Right
    }

    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 10.0f;
    [SerializeField] public int playerHealth;

    private Camera m_Camera;
    private Animator m_Animator;
    private HealthBar healthBar;
    private Dash dash;

    private Vector3 m_Movement;
    private MoveAnim m_moveAnim;

    private float m_camDistanceToPlayer = 15.5f;
    private bool m_isAttacking = false;
    private bool m_isDodging = false;
    private bool m_canDodge = true;
    private bool m_isBlocking = false;
    private bool m_isAbilityActive = false;
    private bool m_isAlive = true;

    public bool IsAttacking { get => m_isAttacking; }

    public GameObject newPlayerObject;  // Used to spawn a new player character on reset

    // UI
    public GameObject mainMenuButton;
    public GameObject retryLevelButton;
    public GameObject deathText;
    public GameObject grayOut;
    public GameObject dashCooldown;


    

    //------------------------------
    // Persistant player data, yay!
    //------------------------------
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    //---------------------------------------------------
    // Start is called before the first frame update
    //---------------------------------------------------
    void Start()
    {
        m_Camera = Camera.main;
        m_Animator = GetComponent<Animator>();
        healthBar = GetComponentInChildren<HealthBar>();
        playerHealth = 100;
        dash = GetComponentInChildren<Dash>();
    }

    //---------------------------------------------------
    // Update is called once per frame
    //---------------------------------------------------

    private void Update()
    {
        if (m_isAlive)
        {
            //-------------------
            //  Player ability
            //-------------------
            
            CheckForAbility();
        }
    }
        void FixedUpdate()
    {
        if (m_isAlive)
        {
            // Only move and rotate if no ability is active
            if (!m_isAbilityActive)
            {
                //-------------------
                //  Player movement
                //-------------------

                MovePlayer();

                //-------------------
                //  Player animation
                //-------------------

                AnimatePlayer();

                //-------------------
                //  Player Rotation
                //-------------------

                RotatePlayer();
            }
        }
    }

    //----------------------------------------------------------------------------------------
    // Checks whether player input a combat ability this frame. If so, play combat
    // animation, use the ability, and block other actions, including movement and rotation.
    //----------------------------------------------------------------------------------------

    private void CheckForAbility()
    {
        // Sword swing attack
        if (Input.GetMouseButtonDown((int)MouseButton.Left) && !m_isAbilityActive)
        {
            Attack();
        }
        // Directional dodge
        if ((Input.GetKeyDown(KeyCode.Space) && !m_isAbilityActive && m_canDodge) || m_isDodging)
        {
            Dodge();
        }
        // Start Block
        if (Input.GetMouseButtonDown((int)MouseButton.Right) && !m_isAbilityActive)
        {
            StartBlock();
        }
        // End Block
        if (Input.GetMouseButtonUp((int)MouseButton.Right) && m_isBlocking)
        {
            EndBlock();
        }
    }

    //-------------------------------------------------------------------------
    // Triggers the attack animation, set the attack and ability flags, and
    // initiate attack cooldown
    //-------------------------------------------------------------------------
    private void Attack()
    {
        m_Animator.SetTrigger("Attack");
        m_isAttacking = true;
        m_isAbilityActive = true;
        StartCoroutine(AttackCooldown());
    }

    //-----------------------------------
    // Cooldown for sword swing attack
    //-----------------------------------
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(ATTACK_DURATION);
        m_isAttacking = false;
        m_isAbilityActive = false;
    }

    //-------------------------------------------------------------------------
    // Handles the directional dodge ability. Applies the animation, starts the
    // dodge cooldown, sets dodge and ability flags, and if already performing
    // the dodge, then applies dodge motion.
    //-------------------------------------------------------------------------
    private void Dodge()
    {
        if (m_isDodging)    // If player is already dodging, continue to apply movement
        {
            if (m_Movement.magnitude == 0)
            {
                transform.position = transform.position - transform.forward * 
                    Time.deltaTime * playerSpeed * DODGE_SPEED_MULTIPLIER;
            }
            else
            {
                transform.position = transform.position + m_Movement *
                    Time.deltaTime * playerSpeed * DODGE_SPEED_MULTIPLIER;
            }
        }
        else                // Otherwise, initiate the dodge
        {
            // Apply dodge animation according to movement direction
            switch (m_moveAnim)
            {
                case MoveAnim.Idle:
                    m_Animator.SetTrigger("DodgeBackward");
                    break;
                case MoveAnim.Forward:
                    m_Animator.SetTrigger("DodgeForward");
                    break;
                case MoveAnim.Backward:
                    m_Animator.SetTrigger("DodgeBackward");
                    break;
                case MoveAnim.Right:
                    m_Animator.SetTrigger("DodgeRight");
                    break;
                case MoveAnim.Left:
                    m_Animator.SetTrigger("DodgeLeft");
                    break;
            }

            m_isDodging = true;
            DODGE_CURRENT_COOLDOWN = 0f;
            dash.UpdateDashCooldown(DODGE_CURRENT_COOLDOWN);
            m_isAbilityActive = true;
            StartCoroutine(DodgeDuration());
        }
    }

    //-----------------------------------
    // Continues dodge for its duration
    //-----------------------------------
    private IEnumerator DodgeDuration()
    {
        yield return new WaitForSeconds(DODGE_DURATION);
        m_isDodging = false;
        m_isAbilityActive = false;
        m_canDodge = false;
        StartCoroutine(DodgeCooldown());
        dashCooldown.SetActive(true);
        InvokeRepeating(nameof(DodgeCooldownCounter), 0, 0.1f);


    }
    private void DodgeCooldownCounter()
    {
        DODGE_CURRENT_COOLDOWN += 0.1f;
        dash.UpdateDashCooldown(DODGE_CURRENT_COOLDOWN);
    }
    public float GetDodgeCooldown()
    {
       return DODGE_CURRENT_COOLDOWN;
    }


    //-----------------------------------
    // Cooldown for directional dodge
    //-----------------------------------
    private IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(DODGE_COOLDOWN);
        m_canDodge = true;
        dashCooldown.SetActive(false);
        CancelInvoke(nameof(DodgeCooldownCounter));

    }

    //---------------------------------------------------------------------------
    // Triggers the raise shield animation and sets flags for starting to block
    //---------------------------------------------------------------------------
    private void StartBlock()
    {
        m_Animator.SetTrigger("StartBlock");
        m_isAbilityActive = true;
        m_isBlocking = true;
    }

    //----------------------------------------------------------------
    // Ends the block animation and clears the flags for blocking
    //----------------------------------------------------------------
    private void EndBlock()
    {
        m_Animator.SetTrigger("EndBlock");
        m_isAbilityActive = false;
        m_isBlocking = false;
    }

    //----------------------------------------------------------------------------------------
    // Moves the player based on the given input then applies movement animation
    //----------------------------------------------------------------------------------------
    private void MovePlayer()
    {
        // Get horizontal and vertical movement value
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Set movement direction and give magnitude of 1
        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        // Move the player
        transform.position = transform.position + m_Movement * Time.deltaTime * playerSpeed;

        // Find the correct animation for running and dodging based on movement
        FindMovementAnimation();
    }

    //----------------------------------------------------------------------------------------
    // Uses vector math to determine which movement animations should be used for running
    // and dodging. Finds which way the character is currently moving relative to the direction
    // they are facing.
    //----------------------------------------------------------------------------------------
    private void FindMovementAnimation()
    {
        // If player isn't moving, apply the idle animation
        if (m_Movement.magnitude == 0f)
        {
            m_moveAnim = MoveAnim.Idle;
            return;
        }

        // Otherwise, get the difference vector between the velocity vector and the forward vector
        Vector3 diffVec = transform.forward - m_Movement;

        // If the magnitude of this vector is less than 2 - Sqrt(2) then player is moving forward.
        // If it's greater than Sqrt(2 + Sqrt(2)) then they're moving back.
        if (diffVec.magnitude <= 2 - Mathf.Sqrt(2))
        {
            m_moveAnim = MoveAnim.Forward;
        }
        else if (diffVec.magnitude >= Mathf.Sqrt(2 + Mathf.Sqrt(2)))
        {
            m_moveAnim = MoveAnim.Backward;
        }
        else
        {
            // Otherwise, player is moving sideways. Now, get the difference vector
            // between the velocity vector the right direction vector, and use the
            // same logic to determine whether they're moving right or left.
            diffVec = transform.right - m_Movement;

            if (diffVec.magnitude < 2 - Mathf.Sqrt(2))
            {
                m_moveAnim = MoveAnim.Right;
            }
            else if (diffVec.magnitude > Mathf.Sqrt(2 + Mathf.Sqrt(2)))
            {
                m_moveAnim = MoveAnim.Left;
            }
        }
    }

    // Apply running animation based on the found movement direction.
    private void AnimatePlayer()
    {
        switch(m_moveAnim)
        {
            case MoveAnim.Idle:
                m_Animator.SetTrigger("IsIdle");
                break;
            case MoveAnim.Forward:
                m_Animator.SetTrigger("IsMovingForward");
                break;
            case MoveAnim.Backward:
                m_Animator.SetTrigger("IsMovingBackward");
                break;
            case MoveAnim.Right:
                m_Animator.SetTrigger("IsMovingRight");
                break;
            case MoveAnim.Left:
                m_Animator.SetTrigger("IsMovingLeft");
                break;
        }
    }

    //--------------------------------------------
    // Rotates the player to look at the mouse
    //--------------------------------------------
    private void RotatePlayer()
    {
        // Get the mouse position in screen space
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mousePos.y / Screen.height * (m_camDistanceToPlayer * 2);    // mouse is 10 units away from camera

        // Get the mouse position in world space and the angle to the mouse
        Vector3 worldMousePos = m_Camera.ScreenToWorldPoint(mousePos);
        float angleRad = Mathf.Atan2(worldMousePos.z - transform.position.z, worldMousePos.x - transform.position.x);
        float angleDeg = (180 / Mathf.PI) * angleRad - 90; // Offset by 90 degrees

        // Rotate player to face mouse in world space
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, -angleDeg, 0f), Time.deltaTime * rotationSpeed);
    }

    //-------------------------------------------------------
    // Disables player controls and plays death animation
    //-------------------------------------------------------
    private void Die()
    {
        m_isAlive = false;
        m_Animator.SetTrigger("Die");
        // Death UI
        retryLevelButton.SetActive(true);
        mainMenuButton.SetActive(true);
        deathText.SetActive(true);
        grayOut.SetActive(true);
    }

    //-----------------------------------------------------------------------
    // Takes damage equal to hitPoints. When health reaches 0, player dies.
    //-----------------------------------------------------------------------
    public void TakeDamage(int hitPoints)
    {
        if (!m_isDodging)
        {
            playerHealth -= hitPoints;
            healthBar.TookDamage();
        }
        
        if (playerHealth <= 0)
        {
            Die();
        }
    }
    // Restart level and reset player functions
    public void RestartLevel()
    {
        SceneManager.LoadScene("StartingRoom");
        retryLevelButton.SetActive(false);
        mainMenuButton.SetActive(false);
        deathText.SetActive(false);
        grayOut.SetActive(false);
        Instantiate(newPlayerObject, new Vector3(0, 0, 0), Quaternion.identity).name = "PlayerObj";
        Destroy(gameObject);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Destroy(gameObject);
    }
}
