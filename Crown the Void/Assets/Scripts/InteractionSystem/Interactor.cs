using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform m_interactionPoint;
    [SerializeField] private float m_interactionRadius = 1.5f;
    [SerializeField] private LayerMask m_interactableMask;
    [SerializeField] private int m_numFound;

    private readonly Collider[] m_interactableColliders = new Collider[3];
    private List<IInteractable> m_interactables = new List<IInteractable>();
    private PlayerController m_pc;

    private void Start()
    {
        m_pc = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        m_numFound = Physics.OverlapSphereNonAlloc(m_interactionPoint.position,
            m_interactionRadius, m_interactableColliders, m_interactableMask);

        if (m_numFound > 0 )
        {
            IInteractable interactable;
            // If we have multiple interactables, find the interactable we should interact with
            if (m_numFound > 1)
            {
                interactable = FindInteractable();
            }
            // If we have one, then just select that interactable
            else
            {
                interactable = m_interactableColliders[0].GetComponent<IInteractable>();
            }

            // If interact button is pressed, interact with the object
            if (interactable != null && Input.GetKeyDown(KeyCode.E) && 
                !interactable.HasInteractedWith && !m_pc.TakingAction)
            {
                interactable.Interact(this);
                m_pc.Interact();
            }
        }
    }

    // Copies objects to an array of interactables, then sorts the array based on priority.
    // Return the highest priority object which hasn't been interacted with yet.
    private IInteractable FindInteractable()
    {
        for (int i = 0; i < m_numFound; i++)
        {
            m_interactables.Add(m_interactableColliders[i].GetComponent<IInteractable>());
        }
        
        m_interactables.Sort(new PriorityComparer());

        foreach (IInteractable i in m_interactables)
        {
            if (!i.HasInteractedWith)
            {
                return i;
            }
        }
        return null;
    }

    // Draw interaction sphere to show interaction radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(m_interactionPoint.position, m_interactionRadius);
    }
}
