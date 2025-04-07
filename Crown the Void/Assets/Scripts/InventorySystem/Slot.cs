using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    [SerializeField] private TextMeshProUGUI m_label;

    public void Set(InventoryItem item)
    {
        m_icon.sprite = item.Data.icon;
        m_label.text = item.Data.displayName;
    }

}
