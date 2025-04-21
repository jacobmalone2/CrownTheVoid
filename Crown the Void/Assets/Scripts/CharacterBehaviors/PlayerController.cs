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
    private const float HEAL_FACTOR = 0.50f;
    private const int ATTACK_BOOST_MULT = 2;
    private const int DAMAGE_DIVIDER = 2;
    private const float BUFF_TIMER_TICK = 0.1f;
    private const float BUFF_DURATION = 30f;
    private const float DODGE_DURATION = 0.25f;
    private const float DODGE_COOLDOWN = 1f;
    public float DODGE_CURRENT_COOLDOWN = 0f;
    private const float DODGE_SPEED_MULTIPLIER = 2.5f;

    private enum MoveAnim   // Enumeration type to define movement directions for animations
    {
        Idle,
        Forward,
        Backward,
        Left,
        Right
    }

    [Header("Player Stats")]
    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 10.0f;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] public int playerHealth;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float throwUpwardForce = 2f;

    [Header("Equipment References")]
    [SerializeField] private GameObject primaryWeapon;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private List<GameObject> items;

    [Header("Projectiles")]
    [SerializeField] private GameObject activeBomb;
    [SerializeField] private GameObject fireStorm;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip throwSound;
    [SerializeField] private AudioClip[] hurtSounds;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip pickUpItemSound;
    [SerializeField] private AudioClip pickUpKeyItemSound;
    [SerializeField] private AudioClip drinkSound;

    private Camera m_Camera;
    private KeyManager m_KeyManager;
    private Animator m_Animator;
    private AudioSource m_AudioSource;
    private InventoryManager m_Inventory;
    private HealthBar healthBar;
    private StatusEffectIcons statusEffectIcons;
    private KeyUI keyUI;
    private Dash dash;

    private Vector3 m_Movement;
    private MoveAnim m_moveAnim;

    private float m_camDistanceToPlayer = 15.5f;

    private bool m_isStopped = true;
    private bool m_isDodging = false;
    private bool m_canDodge = true;
    private bool m_takingAction = false;
    private bool m_isAiming = false;
    private bool m_isShooting = false;
    private bool m_isReloading = false;
    private bool m_isAlive = true;
    private bool m_defenceUp = false;
    private bool m_attackUp = false;
    private float m_boostDamageTimer = BUFF_DURATION;
    private float m_boostDefenceTimer = BUFF_DURATION;
    
    public bool TakingAction { get => m_takingAction; set => m_takingAction = value; }
    public int AttackDamage { get => attackDamage; }
    public bool IsAlive { get => m_isAlive; }
    public bool IsAiming { get => m_isAiming; set => m_isAiming = value; }
    public bool IsShooting { get => m_isShooting; set => m_isShooting = value; }
    public bool IsReloading { get => m_isReloading; set => m_isReloading = value; }

    [Header("Player object reference")]
    public GameObject newPlayerObject;  // Used to spawn a new player character on reset

    [Header("UI References")]
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
        m_AudioSource = GetComponent<AudioSource>();
        m_Inventory = GetComponent<InventoryManager>();
        healthBar = GetComponentInChildren<HealthBar>();
        statusEffectIcons = GetComponentInChildren<StatusEffectIcons>();
        keyUI = GetComponentInChildren<KeyUI>();
        m_KeyManager = GetComponent<KeyManager>();
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

            // Only move if no action is active
            if (!m_takingAction)
            {
                //-------------------
                //  Player movement
                //-------------------

                MovePlayer();

                //----------------------------
                //  Player movement animation
                //----------------------------

                AnimatePlayer();
            }

            // Only rotate if no action is active or if player is aiming as the ranger
            if (!m_takingAction || (m_isAiming && !m_isShooting && !m_isReloading))
            {
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
        if (Input.GetKeyDown(KeyCode.Space) && !m_takingAction && m_canDodge && !isPaused)
        {
            Dodge();
        }
        // Use Item
        if (Input.GetKeyDown(KeyCode.Q) && !m_takingAction && !isPaused)
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

        m_AudioSource.Stop();                   // Stop move sound effect
        m_AudioSource.PlayOneShot(dashSound);   // Play dodge sound effect

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
    // Plays the interact animation and stops movement. If object
    // is an item, play item pick up sound effect
    //-----------------------------------------------------------
    public void Interact(bool isItem, bool isKeyItem)
    {
        m_AudioSource.Stop();       // stop move sound effect
        if (isItem) m_AudioSource.PlayOneShot(pickUpItemSound);
        if (isKeyItem) m_AudioSource.PlayOneShot(pickUpKeyItemSound);
        m_Animator.SetTrigger("Interact");

        m_takingAction = true;
        Invoke(nameof(InteractDuration), INTERACT_DURATION);
    }

    // Called once the interaction duration has passed to allow movement
    private void InteractDuration()
    {
        m_takingAction = false;
    }

    //-----------------------------------------------------------
    // Called by the Interactor when a key is interacted with.
    // Tells the game manager which type of key was picked up and
    // updates the key UI.
    //-----------------------------------------------------------
    public void PickUpKeyItem(KeyItem.KeyType keyType)
    {
        switch (keyType)
        {
            case KeyItem.KeyType.Shadow:
                m_KeyManager.PickUpShadowKey();
                break;
            case KeyItem.KeyType.Blood:
                m_KeyManager.PickUpBloodKey();
                break;
            case KeyItem.KeyType.Void:
                m_KeyManager.PickUpVoidKey();
                break;
        }
        UpdateKeyUI();
    }

    // Updates the key item UI
    public void UpdateKeyUI()
    {
        keyUI.UpdateKeyUI();
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
                m_AudioSource.Stop();       // stop move sound effect
                m_Inventory.RemoveItem();
                HealthPotionEffect();
                break;
            case InventoryManager.ItemType.FuryPotion:
                if (!m_attackUp)
                {
                    m_AudioSource.Stop();
                    m_Inventory.RemoveItem();
                    FuryPotionEffect();
                }
                break;
            case InventoryManager.ItemType.SturdyPotion:
                if (!m_defenceUp)
                {
                    m_AudioSource.Stop();
                    m_Inventory.RemoveItem();
                    SturdyPotionEffect();
                }
                break;
            case InventoryManager.ItemType.Bomb:
                m_AudioSource.Stop();
                m_Inventory.RemoveItem();
                BombEffect();
                break;
            case InventoryManager.ItemType.FireStormTome:
                m_AudioSource.Stop();
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
        m_AudioSource.Stop();       // stop move sound effect
        if (isPaused)
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            isPaused = false;
            // Enable aim line if player is ranger and aiming
            if (gameObject.name.Equals("PlayerObj_Ranger") && m_isAiming)
            {
                GetComponent<LineRenderer>().enabled = true;
            }
        }
        else
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            isPaused = true;
            // Disable aim line if player is ranger
            if (gameObject.name.Equals("PlayerObj_Ranger"))
            {
                GetComponent<LineRenderer>().enabled = false;
            }
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

    // Apply running animation and sound effect based on the found movement direction.
    private void AnimatePlayer()
    {
        switch(m_moveAnim)
        {
            case MoveAnim.Idle:
                m_Animator.SetTrigger("IsIdle");
                if (!m_isStopped)
                {
                    m_isStopped = true;
                    m_AudioSource.Stop();
                }
                break;
            case MoveAnim.Forward:
                m_isStopped = false;
                m_Animator.SetTrigger("IsMovingForward");
                if (!m_AudioSource.isPlaying) m_AudioSource.Play();
                break;
            case MoveAnim.Backward:
                m_isStopped = false;
                m_Animator.SetTrigger("IsMovingBackward");
                if (!m_AudioSource.isPlaying) m_AudioSource.Play();
                break;
            case MoveAnim.Right:
                m_isStopped = false;
                m_Animator.SetTrigger("IsMovingRight");
                if (!m_AudioSource.isPlaying) m_AudioSource.Play();
                break;
            case MoveAnim.Left:
                m_isStopped = false;
                m_Animator.SetTrigger("IsMovingLeft");
                if (!m_AudioSource.isPlaying) m_AudioSource.Play();
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
        m_AudioSource.PlayOneShot(drinkSound);  // Play drinking sound effect

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
        m_AudioSource.PlayOneShot(drinkSound);  // Play drinking sound effect

        attackDamage *= ATTACK_BOOST_MULT;
        m_attackUp = true;
        statusEffectIcons.ShowAttackUp();   // update UI
        
        InvokeRepeating(nameof(BoostDamageTimerTick), 0f, BUFF_TIMER_TICK);
    }

    private void BoostDamageTimerTick()
    {
        // Tick down timer and update UI
        m_boostDamageTimer -= BUFF_TIMER_TICK;
        statusEffectIcons.UpdateAttackUp(m_boostDamageTimer / BUFF_DURATION);

        // When timer runs out, deactivate buff, update UI, and cancel timer and reset for next time
        if (m_boostDamageTimer <= 0)
        {
            attackDamage /= ATTACK_BOOST_MULT;
            m_attackUp = false;
            m_boostDamageTimer = BUFF_DURATION;
            statusEffectIcons.HideAttackUp();

            CancelInvoke(nameof(BoostDamageTimerTick));
        }
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
        m_AudioSource.PlayOneShot(drinkSound);  // Play drinking sound effect

        m_defenceUp = true;
        statusEffectIcons.ShowDefenceUp();  // update UI
        InvokeRepeating(nameof(BoostDefenceTimerTick), 0f, BUFF_TIMER_TICK);
    }

    private void BoostDefenceTimerTick()
    {
        // Tick down timer and update UI
        m_boostDefenceTimer -= BUFF_TIMER_TICK;
        statusEffectIcons.UpdateDefenceUp(m_boostDefenceTimer / BUFF_DURATION);

        // When timer runs out, deactivate buff, update UI, and cancel timer and reset for next time
        if (m_boostDefenceTimer <= 0)
        {
            m_defenceUp = false;
            m_boostDefenceTimer = BUFF_DURATION;
            statusEffectIcons.HideDefenceUp();

            CancelInvoke(nameof(BoostDefenceTimerTick));
        }

        
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

        m_AudioSource.PlayOneShot(throwSound);  // Play throw sound effect

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
        m_AudioSource.PlayOneShot(deathSound);  // Play death sound effect
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

            if (playerHealth <= 0)
            {
                Die();
            }
            else
            {
                // Stop other sounds and play hurt sound effect
                m_AudioSource.Stop();
                m_AudioSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);
            }
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
        
        Instantiate(newPlayerObject, new Vector3(0, 0.2f, 0), Quaternion.identity);
        Destroy(gameObject);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Destroy(gameObject);
    }
    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void CharacterSelect()
    {
        SceneManager.LoadScene("CharacterSelect");
    }
}
