using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangerBehavior : MonoBehaviour
{
    private const float SHOOT_SECS = 0.77f;
    private const float RELOAD_SECS = 1.6f;
    private const float AIM_LINE_DISTANCE = 500f;
    private const int MAX_ARROWS = 3;

    [SerializeField] private float m_shootForce = 0.4f;
    [SerializeField] private int m_arrowCount = 3;
    [SerializeField] private Transform m_shootPoint;
    [SerializeField] private Transform m_aimStartPoint;
    [SerializeField] private GameObject m_projectile;

    private bool m_isAiming;
    private bool m_isShooting;
    private bool m_isReloading;
    private bool m_foundTarget;

    private PlayerController pc;
    private Animator m_Animator;
    private ArrowCountUI m_arrowCountUI;
    private RaycastHit m_target;
    private Vector3[] m_aimPoints;
    private Vector3 m_shootDirection;
    private LineRenderer m_aimLine;

    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();
        m_Animator = GetComponent<Animator>();
        m_aimLine = gameObject.GetComponent<LineRenderer>();
        m_arrowCountUI = gameObject.GetComponentInChildren<ArrowCountUI>();
        m_aimPoints = new Vector3[2];
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.IsAlive)
        {
            // Check for ranger specific action each frame
            CheckForRangerAction();

            // If ranger is aiming and not shooting or reloading, perform a raycast to aim at a target and show aim line
            if (m_isAiming && !m_isShooting && !m_isReloading)
            {
                AimAtTarget();
            }
        }
    }

    private void CheckForRangerAction()
    {
        // Start Aiming
        if (Input.GetMouseButtonDown((int)MouseButton.Right) && !pc.TakingAction)
        {
            StartAiming();
        }
        // Shoot Arrow
        if (Input.GetMouseButtonDown((int)MouseButton.Left) && m_isAiming && m_arrowCount > 0 && !m_isShooting)
        {
            ShootArrow();
        }
        // Reload Arrows
        if (Input.GetKeyDown(KeyCode.R) && m_arrowCount == 0 && (!pc.TakingAction || m_isAiming && !m_isShooting))
        {
            ReloadArrows();
        }
        // Stop Aiming
        if (Input.GetMouseButtonUp((int)MouseButton.Right) && m_isAiming)
        {
            StopAiming();
        }
    }

    //-------------------------------------------------------------------------
    //*************************************************************************
    //                             ACTION METHODS
    //*************************************************************************
    //-------------------------------------------------------------------------

    //----------------------------------------------------------------
    // Starts the aiming animation and sets the flags for aiming
    //----------------------------------------------------------------
    private void StartAiming()
    {
        m_Animator.SetBool("IsAiming", true);
        pc.TakingAction = true;
        m_isAiming = true;
        pc.IsAiming = true;

        m_aimLine.enabled = true;
    }

    //----------------------------------------------------------------
    // Ends the aiming animation and clears the flags for aiming
    //----------------------------------------------------------------
    private void StopAiming()
    {
        // Only clear taking action flag if we're not reloading or shooting
        if (!m_isReloading && !m_isShooting)
            pc.TakingAction = false;

        m_Animator.SetBool("IsAiming", false);
        m_isAiming = false;
        pc.IsAiming = false;

        m_aimLine.enabled = false;
    }

    //----------------------------------------------------------------
    // Starts the shoot animation and launches an arrow toward the 
    // target from the shoot point.
    //----------------------------------------------------------------
    private void ShootArrow()
    {
        m_Animator.SetTrigger("Shoot");
        m_isShooting = true;
        pc.IsShooting = true;

        m_aimLine.enabled = false;

        Quaternion projectileRotation = Quaternion.Euler(-90f, transform.rotation.eulerAngles.y, 0f);

        GameObject projectile = Instantiate(m_projectile, m_shootPoint.position, projectileRotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        projectileRb.AddForce(m_shootDirection * m_shootForce, ForceMode.Impulse);

        // Decrease number of arrows. Once out of arrows, need to reload.
        m_arrowCount--;
        if (m_arrowCount == 0)
        {
            m_arrowCountUI.SetReloadText();
        }
        else
        {
            m_arrowCountUI.SetArrowCount(m_arrowCount);
        }

        Invoke(nameof(ShootReset), SHOOT_SECS);
    }

    private void ShootReset()
    {
        m_isShooting = false;
        pc.IsShooting = false;

        if (m_isAiming) m_aimLine.enabled = true;   // Keep aim line if we're still aiming after shot
        else pc.TakingAction = false;               // Clear taking action flag if we're not still aiming after shot
    }

    //----------------------------------------------------------------
    // Reloads the crossbow with arrows. Plays the animation, refills
    // the arrow count, and updates the UI.
    //----------------------------------------------------------------
    private void ReloadArrows()
    {
        m_Animator.SetTrigger("Reload");
        pc.TakingAction = true;
        pc.IsReloading = true;
        m_isReloading = true;

        m_aimLine.enabled = false;

        Invoke(nameof(ReloadDuration), RELOAD_SECS);
    }

    private void ReloadDuration()
    {
        pc.IsReloading = false;
        m_isReloading = false;
        m_arrowCount = MAX_ARROWS;

        m_arrowCountUI.SetArrowCount(m_arrowCount);

        if (m_isAiming) m_aimLine.enabled = true;   // Keep aim line if we're still aiming after reload
        else pc.TakingAction = false;               // Clear taking action flag if we're not still aiming after reload
    }

    //-------------------------------------------------------------------------
    //*************************************************************************
    //                     METHODS FOR AIMING AND RAYCASTING
    //*************************************************************************
    //-------------------------------------------------------------------------

    //----------------------------------------------------------------
    // Uses a raycast to aim at a target and set a shooting direction
    // towards that target. If no target is found, sets the shooting
    // direction forward.
    //----------------------------------------------------------------
    private void AimAtTarget()
    {
        if (Physics.Raycast(m_aimStartPoint.position, transform.forward, out m_target, AIM_LINE_DISTANCE))
        {
            m_shootDirection = (m_target.point - m_shootPoint.position).normalized;
            m_foundTarget = true;
        }
        else
        {
            m_shootDirection = transform.forward;
            m_foundTarget = false;
        }
        DrawAimLine();
    }

    //-----------------------------------------------------------
    // Draws an aim line created by the raycast from the player
    // to the target.
    //-----------------------------------------------------------
    private void DrawAimLine()
    {
        m_aimPoints[0] = m_aimStartPoint.position;

        if (m_foundTarget)
            m_aimPoints[1] = m_target.point;
        else 
            m_aimPoints[1] = m_aimStartPoint.position + transform.forward * AIM_LINE_DISTANCE;

        m_aimLine.SetPositions(m_aimPoints);

        if (m_foundTarget && (m_target.collider.gameObject.CompareTag("Enemy") || 
            m_target.collider.gameObject.CompareTag("Sword")))
        {
            m_aimLine.startColor = Color.red;
            m_aimLine.endColor = Color.red;
        }
        else
        {
            m_aimLine.startColor = Color.yellow;
            m_aimLine.endColor = Color.yellow;
        }
    }
}
