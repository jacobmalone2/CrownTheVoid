using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private GameObject m_slotPrefab; // Prefab for the inventory slot
    [SerializeField] private GameObject[] slots; // Reference to the inventory panel
    private InventoryManager m_inventoryManager; // Reference to the InventoryManager
    private int invSize; // Size of the inventory
    private Color32 selectedColor = new Color32(88, 19, 125, 209);
    private Color32 defaultColor = new Color32(0, 0, 0, 209);
    public void Start()
    {
        m_inventoryManager = gameObject.GetComponentInParent<InventoryManager>();
    }
    public void SelectedItem()
    {
        // set to default color
        for (int i = 0; i < slots.Length; i++)
        {
            if (i != m_inventoryManager.m_equippedItemIndex)
            {
                slots[i].GetComponentInChildren<Image>().color = defaultColor;
            }
        }
        slots[m_inventoryManager.m_equippedItemIndex].GetComponentInChildren<Image>().color = selectedColor;        
    }

    public void AddInventorySlot(InventoryItem item)
    {
        invSize = m_inventoryManager.Inventory.Count;
        //obj.transform.SetParent(transform, false);
        Slot slot = slots[invSize - 1].GetComponent<Slot>();
        Debug.Log("Slot: " + slot);
        slot.Set(item);
        //slots[invSize - 1] = slot.gameObject;
        // Set the slot to be active
        slots[invSize - 1].SetActive(true);
    }
    public void RemoveInventorySlot()
    {
        // Deactivate the last slot in the inventory
        slots[m_inventoryManager.m_equippedItemIndex].SetActive(false);
        invSize = m_inventoryManager.Inventory.Count;
        //Shift the slots down
        for (int i = m_inventoryManager.m_equippedItemIndex; i < invSize - 1; i++)
        {
            InventoryItem item = m_inventoryManager.Inventory[i + 1];
            Slot slot = slots[invSize - 1].GetComponent<Slot>();
            slot.Set(item);

        }

    }
}
