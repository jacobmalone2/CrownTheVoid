using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour, IInteractable
{
    private readonly int m_interactPriority = 0;
    private string m_interactionPrompt = "Health Potion";
    private bool m_hasInteractedWith = false;

    [SerializeField] private InventoryItemData HealthPotionData;

    private InteractionPopUpBehavior m_popUp;

    private void Start()
    {
        m_popUp = GetComponentInChildren<InteractionPopUpBehavior>();
    }

    public string InteractionPrompt => m_interactionPrompt;

    public bool HasInteractedWith => m_hasInteractedWith;

    public int InteractPriority => m_interactPriority;

    public bool Interact(Interactor interactor)
    {
        InventoryManager interactorInventory = interactor.GetComponent<InventoryManager>();

        m_hasInteractedWith = true;
        interactorInventory.AddItem(HealthPotionData);
        Destroy(gameObject);
        return m_hasInteractedWith;
    }

    public void ShowPopUp(string prompt)
    {
        m_popUp.ShowPopUp(prompt);
    }
}
