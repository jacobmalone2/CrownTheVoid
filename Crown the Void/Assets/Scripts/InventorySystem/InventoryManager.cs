using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private int m_maxItems = 5;             // Max items inventory can hold
    private int m_equippedItemIndex = 0;     // The index position of the currently equipped item
    private int m_numItems = 0;              // Number of items in inventory
    public InventoryUIManager InventoryUIManager; // Reference to the InventoryUIManager

    public enum ItemType        // Enumeration type to represent types of items
    {
        NullItem,
        HealthPotion,
        FuryPotion,
        SturdyPotion,
        Bomb,
        FireStormTome
    }

    private InventoryItem[] m_inventory;

    public  InventoryItem[] Inventory { get => m_inventory; }
    public int NumItems { get => m_numItems; }
    public int EquippedItemIndex { get => m_equippedItemIndex; }

    private void Awake()
    {
        m_inventory = new InventoryItem[m_maxItems];
        InventoryUIManager = gameObject.GetComponentInChildren<InventoryUIManager>();
    }

    // Check for item swap each frame
    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            SwapForward();
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            SwapBackward();
    }

    // If inventory isn't full, adds the item corresponding to itemData to the inventory list.
    // If it is full, drop the currently held item and replace it with new item.
    public void AddItem(InventoryItemData itemData)
    {
        InventoryItem newItem = new InventoryItem(itemData);
        if (m_numItems < m_maxItems)
        {
            for (int i = 0; i < m_inventory.Length; i++)
            {
                if (m_inventory[i] == null)
                {
                    m_inventory[i] = newItem;
                    m_numItems++;
                    InventoryUIManager.AddInventorySlot(newItem, i);    // Add the item to the inventory UI
                    break;
                }
            }
        }
        else
        {
            DropItem();
            m_inventory[m_equippedItemIndex] = newItem;
            InventoryUIManager.AddInventorySlot(newItem, m_equippedItemIndex); // Add the item to the inventory UI
        }
    }

    // Returns the type of item currently equipped
    public ItemType GetEquippedItem()
    {
        try
        {
            InventoryItemData equippedItemData = m_inventory[m_equippedItemIndex].Data;

            if (equippedItemData.id.Equals("HealthPotion"))
            {
                return ItemType.HealthPotion;
            }
            else if (equippedItemData.id.Equals("FuryPotion"))
            {
                return ItemType.FuryPotion;
            }
            else if (equippedItemData.id.Equals("SturdyPotion"))
            {
                return ItemType.SturdyPotion;
            }
            else if (equippedItemData.id.Equals("Bomb"))
            {
                return ItemType.Bomb;
            }
            else if (equippedItemData.id.Equals("FireStormTome"))
            {
                return ItemType.FireStormTome;
            }

            return ItemType.NullItem;
        }
        catch (NullReferenceException)
        {

            return ItemType.NullItem;
        }
    }
    
    // Removes the currently equipped item from the inventory list
    public void RemoveItem()
    {
        m_inventory[m_equippedItemIndex] = null;
        m_numItems--;

        // Remove the item from the inventory UI
        InventoryUIManager.RemoveInventorySlot(m_equippedItemIndex);
    }

    // Swaps the currently equipped item forward by one slot
    private void SwapForward()
    {
        int prevSelectedIndx = m_equippedItemIndex;

        m_equippedItemIndex++;
        if (m_equippedItemIndex >= m_maxItems)
        {
            m_equippedItemIndex = 0;
        }

        // Set the selected item in the UI
        InventoryUIManager.SelectedItem(prevSelectedIndx);
    }

    // Swaps the currently equipped item backward by one slot
    private void SwapBackward()
    {
        int prevSelectedIndx = m_equippedItemIndex;

        m_equippedItemIndex--;
        if (m_equippedItemIndex < 0)
        {
            m_equippedItemIndex = m_maxItems - 1;
        }

        // Set the equipped item in the UI
        InventoryUIManager.SelectedItem(prevSelectedIndx);
    }

    // Drops the currently held item in front of the player and removes it from the inventory
    private void DropItem()
    {
        GameObject droppedItem = m_inventory[m_equippedItemIndex].Data.prefab;

        Instantiate(droppedItem, gameObject.transform.position +
            gameObject.transform.forward, droppedItem.transform.rotation);

        m_inventory[m_equippedItemIndex] = null;
    }
    

}

