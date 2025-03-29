using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject itemDrop;

    private readonly int m_interactPriority = 1;
    private string m_interactionPrompt = "Open Chest";
    private bool m_hasInteractedWith = false;

    private Animator m_animator;
    private InteractionPopUpBehavior m_popUp;

    public string InteractionPrompt => m_interactionPrompt;
    public bool HasInteractedWith => m_hasInteractedWith;
    public int InteractPriority => m_interactPriority;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_popUp = GetComponentInChildren<InteractionPopUpBehavior>();
    }

    // Called when this object is interacted with. Plays open animation, signals
    // that it's been interacted with, and drops the item it holds
    public bool Interact(Interactor interactor)
    {
        m_animator.SetTrigger("Open");
        m_hasInteractedWith = true;
        Invoke(nameof(DropItem), 1f);
        return true;
    }

    // Drops currently held item after a second
    private void DropItem()
    {
        Instantiate(itemDrop, transform.position + transform.forward * 0.25f, itemDrop.transform.rotation);
    }

    public void ShowPopUp(string prompt)
    {
        m_popUp.ShowPopUp(prompt);
    }
}
