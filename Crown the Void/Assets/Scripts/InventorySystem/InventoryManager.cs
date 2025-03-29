using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private int m_maxItems = 5;         // Max items inventory can hold
    private int m_equippedItemIndex = 0;    // The index position of the currently equipped item

    public enum ItemType        // Enumeration type to represent types of items
    {
        NullItem,
        HealthPotion
    }

    private List<InventoryItem> m_inventory;

    public List<InventoryItem> Inventory { get => m_inventory; }

    private void Awake()
    {
        m_inventory = new List<InventoryItem>();
    }

    // If inventory isn't full, adds the item corresponding to itemData to the inventory list.
    // If it is full, drop the currently held item and replace it with new item.
    public void AddItem(InventoryItemData itemData)
    {
        InventoryItem newItem = new InventoryItem(itemData);
        if (m_inventory.Count < m_maxItems)
        {
            m_inventory.Add(newItem);
        }
        else
        {
            DropItem();
            m_inventory.Insert(m_equippedItemIndex, newItem);
        }
        foreach (InventoryItem item in m_inventory)
        {
            Debug.Log(item.Data.id);
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

            return ItemType.NullItem;
        }
        catch (System.ArgumentOutOfRangeException)
        {

            return ItemType.NullItem;
        }
    }

    // Removes the currently equipped item from the inventory list
    public void RemoveItem()
    {
        m_inventory.RemoveAt(m_equippedItemIndex);

        // Adjust equipped item index if it's out of bounds and we still have items left
        if (m_equippedItemIndex >= m_inventory.Count && m_equippedItemIndex > 0)
        {
            m_equippedItemIndex--;
        }
        foreach (InventoryItem item in m_inventory)
        {
            Debug.Log(item.Data.id);
        }
    }

    // Swaps the currently equipped item by "interval" number of slots. Swaps forward if positive,
    // or backward if negative
    public void SwapItem(int interval)
    {

    }

    // Drops the currently held item in front of the player and removes it from the inventory
    private void DropItem()
    {
        GameObject droppedItem = m_inventory[m_equippedItemIndex].Data.prefab;

        Instantiate(droppedItem, gameObject.transform.position + 
            gameObject.transform.forward, droppedItem.transform.rotation);

        m_inventory.RemoveAt(m_equippedItemIndex);
    }
}