using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] slots;              // Reference to inventory slots
    private InventoryManager m_inventoryManager;        // Reference to the InventoryManager
    

    public void Start()
    {
        m_inventoryManager = gameObject.GetComponentInParent<InventoryManager>();

        // Highlight first slot when we start
        slots[m_inventoryManager.EquippedItemIndex].GetComponent<Slot>().SelectSlot();
    }

    // Highlights the selected item inventory slot
    public void SelectedItem(int prevSelectedIndx)
    {
        // Set currently selected slot color
        slots[m_inventoryManager.EquippedItemIndex].GetComponent<Slot>().SelectSlot();

        // Reset previously selected slot color
        slots[prevSelectedIndx].GetComponent<Slot>().DeselectSlot();
    }

    // Adds the new item's data to it's inventory slot and activates it
    public void AddInventorySlot(InventoryItem item, int slotPosition)
    {
        // Configure item data for slot
        slots[slotPosition].GetComponent<Slot>().Set(item);
    }

    // Deactivates the the inventory slot for the removed item
    public void RemoveInventorySlot(int slotPosition)
    {
        // Deactivate the last slot in the inventory
        slots[slotPosition].GetComponent<Slot>().RemoveItem();
    }
}
