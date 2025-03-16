using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour, IInteractable
{
    private readonly int m_interactPriority = 0;
    private string m_interactionPrompt = "E";
    private bool m_hasInteractedWith = false;

    [SerializeField] private InventoryItemData HealthPotionData;

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
}
