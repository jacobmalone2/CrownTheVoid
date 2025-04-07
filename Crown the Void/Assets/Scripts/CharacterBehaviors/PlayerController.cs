using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private const float INTERACT_DURATION = 1.3f;
    private const float ITEM_USE_DURATION = 1.6f;
    private const float ITEM_THROW_DURATION = 1.36f;
    private const float SPELL_CAST_DURATION = 4.2f;
    private const float TIME_TO_DRINK = 0.6f;
    private const float HEAL_FACTOR = 0.25f;
    private const int ATTACK_BOOST_MULT = 2;
    private const int DAMAGE_DIVIDER = 2;
    private const float BUFF_DURATION = 15f;
    private const float DODGE_DURATION = 0.25f;
    private const float DODGE_COOLDOWN = 1f;
    public float DODGE_CURRENT_COOLDOWN = 0f;
    private const float DODGE_SPEED_MULTIPLIER = 3f;

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
    [SerializeField] private int maxHealth = 100;
    [SerializeField] public int playerHealth;
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float throwUpwardForce = 2f;
    [SerializeField] private GameObject primaryWeapon;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private List<GameObject> items;
    [SerializeField] private GameObject activeBomb;
    [SerializeField] private GameObject fireStorm;

    private Camera m_Camera;
    private Animator m_Animator;
    private InventoryManager m_Inventory;
    private HealthBar healthBar;
    private Dash dash;

    private Vector3 m_Movement;
    private MoveAnim m_moveAnim;

    private float m_camDistanceToPlayer = 15.5f;
    
    private bool m_isDodging = false;
    private bool m_canDodge = true;
    private bool m_takingAction = false;
    private bool m_isAlive = true;
    private bool m_defenceUp = false;
    private bool m_attackUp = false;
    
    public bool TakingAction { get => m_takingAction; set => m_takingAction = value; }
    public int AttackDamage { get => attackDamage; }
    public bool IsAlive { get => m_isAlive; }

    public GameObject newPlayerObject;  // Used to spawn a new player character on reset

    // UI
    public GameObject mainMenuButton;
    public GameObject retryLevelButton;
    public GameObject deathText;
    public GameObject grayOut;
    public GameObject dashCooldown;
    public GameObject pauseMenu;

    public bool isPaused = false;



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
        m_Inventory = GetComponent<InventoryManager>();
        healthBar = GetComponentInChildren<HealthBar>();
        playerHealth = maxHealth;
        dash = GetComponentInChildren<Dash>();
        PauseGame(isPaused = true);
    }

    //---------------------------------------------------
    // Update is called once per frame
    //---------------------------------------------------
    private void Update()
    {
        if (m_isAlive)
        {
            // Check for universal action each frame

            CheckForAction();
        }
    }
    void FixedUpdate()
    {
        if (m_isAlive)
        {
            // If player is already dodging, continue to apply movement
            if (m_isDodging)
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

            // Only move and rotate if no ability is active
            if (!m_takingAction)
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

    //-------------------------------------------------------------------------
    //*************************************************************************
    //                             ACTION METHODS
    //*************************************************************************
    //-------------------------------------------------------------------------


    //----------------------------------------------------------------------------------------
    // Checks whether player input an action this frame. If so, calls a method to handle
    // the action.
    //----------------------------------------------------------------------------------------
    private void CheckForAction()
    {
        // Directional dodge
        if ((Input.GetKeyDown(KeyCode.Space) && !m_takingAction && m_canDodge))
        {
            Dodge();
        }
        // Use Item
        if (Input.GetKeyDown(KeyCode.Q) && !m_takingAction)
        {
            UseItem();
        }
        // Pause game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(isPaused);
        }
    }

    //-------------------------------------------------------------------------
    // Handles the directional dodge ability. Applies the animation, starts the
    // dodge cooldown, sets dodge and ability flags, and if already performing
    // the dodge, then applies dodge motion.
    //-------------------------------------------------------------------------
    private void Dodge()
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
        m_takingAction = true;
        StartCoroutine(DodgeDuration());
    }

    // Continues dodge for its duration
    private IEnumerator DodgeDuration()
    {
        yield return new WaitForSeconds(DODGE_DURATION);
        m_isDodging = false;
        m_takingAction = false;
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

    // Cooldown for directional dodge
    private IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(DODGE_COOLDOWN);
        m_canDodge = true;
        dashCooldown.SetActive(false);
        CancelInvoke(nameof(DodgeCooldownCounter));

    }

    //-----------------------------------------------------------
    // Called by the Interactor when interacting with an object.
    // Plays the interact animation and stops movement.
    //-----------------------------------------------------------
    public void Interact()
    {
        m_Animator.SetTrigger("Interact");
        m_takingAction = true;
        Invoke(nameof(InteractDuration), INTERACT_DURATION);
    }

    // Called once the interaction duration has passed to allow movement
    private void InteractDuration()
    {
        m_takingAction = false;
    }

    //-------------------------------------------------------------------------------------
    // Uses the currently equipped item in the inventory. Determines which item effect to
    // activate according to the equipped item, then removes the item from inventory.
    //-------------------------------------------------------------------------------------
    private void UseItem()
    {
        InventoryManager.ItemType item = m_Inventory.GetEquippedItem();
        switch (item)
        {
            case InventoryManager.ItemType.NullItem:
                break;
            case InventoryManager.ItemType.HealthPotion:
                m_Inventory.RemoveItem();
                HealthPotionEffect();
                break;
            case InventoryManager.ItemType.FuryPotion:
                if (!m_attackUp)
                {
                    m_Inventory.RemoveItem();
                    FuryPotionEffect();
                }
                break;
            case InventoryManager.ItemType.SturdyPotion:
                if (!m_defenceUp)
                {
                    m_Inventory.RemoveItem();
                    SturdyPotionEffect();
                }
                break;
            case InventoryManager.ItemType.Bomb:
                m_Inventory.RemoveItem();
                BombEffect();
                break;
            case InventoryManager.ItemType.FireStormTome:
                m_Inventory.RemoveItem();
                FireStormEffect();
                break;
        }
    }

    //---------------------------------------------
    // Pauses the game and reveals the pause menu
    //---------------------------------------------
    public void PauseGame(bool pause)
    {
        if (isPaused)
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            isPaused = false;

        }
        else
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            isPaused = true;
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    //-------------------------------------------------------------------------
    //*************************************************************************
    //                          PLAYER MOVEMENT METHODS
    //*************************************************************************
    //-------------------------------------------------------------------------


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
            else
            {
                m_moveAnim = MoveAnim.Forward;
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

    //-------------------------------------------------------------------------
    //*************************************************************************
    //                           ITEM EFFECT FUNCTIONS
    //*************************************************************************
    //-------------------------------------------------------------------------

    //------------------------------------------------------------------
    // Triggers the health potion effect. Plays item use animation,
    // then heals player after a short delay.
    //------------------------------------------------------------------
    private void HealthPotionEffect()
    {
        m_Animator.SetTrigger("UseItem");
        m_takingAction = true;

        primaryWeapon.SetActive(false);
        items[0].SetActive(true);

        Invoke(nameof(Heal), TIME_TO_DRINK);
        StartCoroutine(ItemUseDuration(0));
    }

    private void Heal()
    {
        playerHealth += (int)(HEAL_FACTOR * maxHealth);
        if (playerHealth > maxHealth) playerHealth = maxHealth;
        healthBar.TookDamage(); // Update health bar
    }

    //------------------------------------------------------------------
    // Triggers the fury potion effect. Plays item use animation,
    // then boosts player attack damage for a duration.
    //------------------------------------------------------------------
    private void FuryPotionEffect()
    {
        m_Animator.SetTrigger("UseItem");
        m_takingAction = true;

        primaryWeapon.SetActive(false);
        items[1].SetActive(true);

        Invoke(nameof(BoostDamage), TIME_TO_DRINK);
        StartCoroutine(ItemUseDuration(1));
    }

    private void BoostDamage()
    {
        attackDamage *= ATTACK_BOOST_MULT;
        m_attackUp = true;
        
        Invoke(nameof(BoostDamageDuration), BUFF_DURATION);
        Debug.Log("Attack up");
    }

    private void BoostDamageDuration()
    {
        attackDamage /= ATTACK_BOOST_MULT;
        m_attackUp = false;
        Debug.Log("Attack back to normal");
    }

    //------------------------------------------------------------------
    // Triggers the sturdy potion effect. Plays item use animation,
    // then negates a portion of incoming damage.
    //------------------------------------------------------------------
    private void SturdyPotionEffect()
    {
        m_Animator.SetTrigger("UseItem");
        m_takingAction = true;

        primaryWeapon.SetActive(false);
        items[2].SetActive(true);

        Invoke(nameof(BoostDefence), TIME_TO_DRINK);
        StartCoroutine(ItemUseDuration(2));
    }

    private void BoostDefence()
    {
        m_defenceUp = true;
        Invoke(nameof(BoostDefenceDuration), BUFF_DURATION);
        Debug.Log("Defence up");
    }

    private void BoostDefenceDuration()
    {
        m_defenceUp = false;
        Debug.Log("Defence back to normal");
    }

    //------------------------------------------------------------------
    // Triggers the bomb item effect. Starts the throw animation, and
    // launches an active bomb projectile.
    //------------------------------------------------------------------
    private void BombEffect()
    {
        m_Animator.SetTrigger("ThrowItem");
        m_takingAction = true;

        primaryWeapon.SetActive(false);
        items[3].SetActive(true);

        StartCoroutine(ItemThrowDuration());
    }

    private void FireStormEffect()
    {
        m_Animator.SetTrigger("CastSpell");
        m_takingAction = true;

        Instantiate(fireStorm, transform.position, Quaternion.identity);
        m_defenceUp = true;
        primaryWeapon.SetActive(false);
        items[4].SetActive(true);

        StartCoroutine(SpellCastDuration(4));
    }

    public void ThrowItem()
    {
        // Instantiate an active bomb, then apply the throwing force to its rigidbody
        GameObject bomb = Instantiate(activeBomb, throwPoint.position, Quaternion.identity);
        Rigidbody bombRb = bomb.GetComponent<Rigidbody>();
        Vector3 forceToAdd = transform.forward * throwForce + transform.up * throwUpwardForce;

        bombRb.AddForce(forceToAdd, ForceMode.Impulse);

        items[3].SetActive(false);
    }

    private IEnumerator ItemUseDuration(int itemIndex)
    {
        yield return new WaitForSeconds(ITEM_USE_DURATION);
        primaryWeapon.SetActive(true);
        items[itemIndex].SetActive(false);
        m_takingAction = false;
    }

    private IEnumerator ItemThrowDuration()
    {
        yield return new WaitForSeconds(ITEM_THROW_DURATION);
        primaryWeapon.SetActive(true);
        m_takingAction = false;
    }

    private IEnumerator SpellCastDuration(int itemIndex)
    {
        yield return new WaitForSeconds(SPELL_CAST_DURATION);
        primaryWeapon.SetActive(true);
        items[itemIndex].SetActive(false);
        m_takingAction = false;
        m_defenceUp = false;
    }

    //-------------------------------------------------------------------------
    //*************************************************************************
    //                              MISC FUNCTIONS
    //*************************************************************************
    //-------------------------------------------------------------------------


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
            if (m_defenceUp) hitPoints /= DAMAGE_DIVIDER;

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
        playerHealth = maxHealth;
        healthBar.TookDamage();
        
        Instantiate(newPlayerObject, new Vector3(0, 0, 0), Quaternion.identity).name = "PlayerObj";
        Destroy(gameObject);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Destroy(gameObject);
    }
}
