using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    [SerializeField] private TextMeshProUGUI m_label;

    private Color32 selectedColor = new Color32(78, 30, 79, 203);
    private Color32 defaultColor = new Color32(0, 0, 0, 0);

    // Sets a new item into a slot
    public void Set(InventoryItem item)
    {
        m_icon.sprite = item.Data.icon;
        m_label.text = item.Data.displayName;

        m_icon.gameObject.SetActive(true);
    }

    // Highlights the slot and shows label
    public void SelectSlot()
    {
        GetComponent<Image>().color = selectedColor;
        m_label.gameObject.SetActive(true);
    }

    // Resets slot color and hides label
    public void DeselectSlot()
    {
        GetComponent<Image>().color = defaultColor;
        m_label.gameObject.SetActive(false);
    }

    // Hides item icon and resets label
    public void RemoveItem()
    {
        m_icon.gameObject.SetActive(false);
        m_label.text = string.Empty;
    }
}
