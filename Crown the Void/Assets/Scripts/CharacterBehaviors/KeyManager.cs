using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    private bool hasShadowKey = false;
    private bool hasBloodKey = false;
    private bool hasVoidKey = false;

    private KeyUI keyUI;

    public bool HasVoidKey { get => hasVoidKey; }
    public bool HasBloodKey { get => hasBloodKey; }
    public bool HasShadowKey { get => hasShadowKey; }

    // Start is called before the first frame update
    void Start()
    {
        keyUI = GetComponentInChildren<KeyUI>();

        if (PlayerPrefs.HasKey("hasShadowKey"))
        {
            LoadKeyData();
        }
    }

    // Pick up shadow key and assign material to key slot
    public void PickUpShadowKey()
    {
        hasShadowKey = true;
        SaveKeyData();

        GameObject.Find("GameManager").GetComponent<GameManager>().FillShadowKeySlot();
    }

    // Pick up Blood key and assign material to key slot
    public void PickUpBloodKey()
    {
        hasBloodKey = true;
        SaveKeyData();

        GameObject.Find("GameManager").GetComponent<GameManager>().FillBloodKeySlot();
    }

    // Pick up shadow key and assign material to key slot
    public void PickUpVoidKey()
    {
        hasVoidKey = true;
        SaveKeyData();

        GameObject.Find("GameManager").GetComponent<GameManager>().FillVoidKeySlot();
    }

    // Resets key save data
    public void ClearKeyData()
    {
        PlayerPrefs.SetInt("hasShadowKey", 0);
        PlayerPrefs.SetInt("hasBloodKey", 0);
        PlayerPrefs.SetInt("hasVoidKey", 0);
    }


    // Saves current key data
    private void SaveKeyData()
    {
        if (hasShadowKey == true) PlayerPrefs.SetInt("hasShadowKey", 1);
        else PlayerPrefs.SetInt("hasShadowKey", 0);

        if (hasBloodKey == true) PlayerPrefs.SetInt("hasBloodKey", 1);
        else PlayerPrefs.SetInt("hasBloodKey", 0);

        if (hasVoidKey == true) PlayerPrefs.SetInt("hasVoidKey", 1);
        else PlayerPrefs.SetInt("hasVoidKey", 0);
    }

    // Loads key save data
    private void LoadKeyData()
    {
        if (PlayerPrefs.GetInt("hasShadowKey") == 1) hasShadowKey = true;
        else hasShadowKey = false;

        if (PlayerPrefs.GetInt("hasBloodKey") == 1) hasBloodKey = true;
        else hasBloodKey = false;

        if (PlayerPrefs.GetInt("hasVoidKey") == 1) hasVoidKey = true;
        else hasVoidKey = false;

        keyUI.UpdateKeyUI();
    }
}
