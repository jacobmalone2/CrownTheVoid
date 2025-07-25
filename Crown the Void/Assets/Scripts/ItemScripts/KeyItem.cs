using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : MonoBehaviour, IInteractable
{
    public enum KeyType
    {
        Shadow,
        Blood,
        Void
    }

    private readonly bool m_isItem = false;
    private readonly bool m_isKeyItem = true;
    private readonly int m_interactPriority = 0;
    [SerializeField] private string m_interactionPrompt;
    private bool m_hasInteractedWith = false;

    private InteractionPopUpBehavior m_popUp;
    [SerializeField] private KeyType typeOfKey;

    public int InteractPriority => m_interactPriority;
    public string InteractionPrompt => m_interactionPrompt;
    public bool HasInteractedWith => m_hasInteractedWith;
    public bool IsItem => m_isItem;
    public bool IsKeyItem => m_isKeyItem;
    public KeyType TypeOfKey { get => typeOfKey; }

    private void Start()
    {
        m_popUp = GetComponentInChildren<InteractionPopUpBehavior>();
    }

    public bool Interact(Interactor interactor)
    {
        m_hasInteractedWith = true;
        Destroy(gameObject);
        return m_hasInteractedWith;
    }

    public void ShowPopUp(string prompt)
    {
        m_popUp.ShowPopUp(prompt);
    }
}
