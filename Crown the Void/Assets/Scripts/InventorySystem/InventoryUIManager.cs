using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private GameObject m_slotPrefab; // Prefab for the inventory slot
    public void Start()
    {
        //InventoryManager.current.onInventoryChangedEvent += onUpdateInventory;

    }
    private void onUpdateInventory()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        DrawInventory();
    }
    public void DrawInventory()
    {
        //foreach (InventoryItem item in InventoryManager.current.inventory)
        //{
        //    GameObject slot = Instantiate(item.Data.prefab, transform);
        //    slot.GetComponent<Slot>().Set(item);
        //}
    }
    public void AddInventorySlot(InventoryItem item)
    {
        GameObject obj = Instantiate(m_slotPrefab);
        obj.transform.SetParent(transform, false);

        Slot slot = obj.GetComponent<Slot>();
        slot.Set(item);
    }
}
