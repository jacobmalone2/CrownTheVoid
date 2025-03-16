using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to represent an instanced inventory item
[Serializable]
public class InventoryItem
{
    private InventoryItemData m_data;

    public InventoryItemData Data { get => m_data; }

    public InventoryItem(InventoryItemData source)
    {
        m_data = source;
    }
}
