using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangerBehavior : MonoBehaviour
{
    private const float SHOOT_DURATION = 0.77f;
    private const float AIM_LINE_DISTANCE = 500f;

    [SerializeField] private Transform m_shootPoint;
    [SerializeField] private Transform m_aimStartPoint;

    private bool m_isAiming;
    private bool m_isShooting;

    private PlayerController pc;
    private Animator m_Animator;
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
        m_aimPoints = new Vector3[2];
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.IsAlive)
        {
            // Check for ranger specific action each frame
            CheckForRangerAction();

            // If ranger is aiming and not shooting, perform a raycast to aim at a target and show aim line
            if (m_isAiming && !m_isShooting)
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
        if (Input.GetMouseButtonDown((int)MouseButton.Left) && m_isAiming && !m_isShooting)
        {
            ShootArrow();
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
        m_Animator.SetBool("IsAiming", false);
        pc.TakingAction = false;
        m_isAiming = false;
        pc.IsAiming = false;

        m_aimLine.enabled = false;
    }

    private void ShootArrow()
    {
        m_Animator.SetTrigger("Shoot");
        m_isShooting = true;
        pc.IsShooting = true;

        m_aimLine.enabled = false;

        Invoke(nameof(ShootReset), SHOOT_DURATION);
    }

    private void ShootReset()
    {
        m_isShooting = false;
        pc.IsShooting = false;

        if (m_isAiming) m_aimLine.enabled = true;
    }

    //-------------------------------------------------------------------------
    //*************************************************************************
    //                     METHODS FOR AIMING AND RAYCASTING
    //*************************************************************************
    //-------------------------------------------------------------------------

    private void AimAtTarget()
    {
        if (Physics.Raycast(m_aimStartPoint.position, transform.forward, out m_target, AIM_LINE_DISTANCE))
        {
            m_shootDirection = (m_target.point - m_shootPoint.position).normalized;
            Debug.Log("found target");

            // Draw aim line
            DrawAimLine(m_target.point);
        }
        else
            DrawAimLine(m_aimStartPoint.position + transform.forward * AIM_LINE_DISTANCE);
    }

    private void DrawAimLine(Vector3 endPoint)
    {
        m_aimPoints[0] = m_aimStartPoint.position;
        m_aimPoints[1] = endPoint;
        m_aimLine.SetPositions(m_aimPoints);

        if (m_target.collider.gameObject.CompareTag("Enemy"))
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
