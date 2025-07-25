using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool hasShadowKey = false;
    private bool hasBloodKey = false;
    private bool hasVoidKey = false;

    [SerializeField] private GameObject ShadowKeyObject;
    [SerializeField] private GameObject BloodKeyObject;
    [SerializeField] private GameObject VoidKeyObject;
    [SerializeField] private GameObject ShadowKeySlot;
    [SerializeField] private GameObject BloodKeySlot;
    [SerializeField] private GameObject VoidKeySlot;
    [SerializeField] private Material ShadowKeyMaterial;
    [SerializeField] private Material BloodKeyMaterial;
    [SerializeField] private Material VoidKeyMaterial;

    private void Start()
    {
        if (PlayerPrefs.HasKey("hasShadowKey"))
        {
            LoadKeyData();
        }

        SpawnKeys();
        FillKeySlots();
    }

    public void FillShadowKeySlot()
    {
        ShadowKeySlot.GetComponent<Renderer>().material = ShadowKeyMaterial;
    }

    public void FillBloodKeySlot()
    {
        BloodKeySlot.GetComponent<Renderer>().material = BloodKeyMaterial;
    }

    public void FillVoidKeySlot()
    {
        VoidKeySlot.GetComponent<Renderer>().material = VoidKeyMaterial;
    }

    // Fills key slots according to save data
    private void FillKeySlots()
    {
        if (hasShadowKey) ShadowKeySlot.GetComponent<Renderer>().material = ShadowKeyMaterial;
        if (hasBloodKey) BloodKeySlot.GetComponent<Renderer>().material = BloodKeyMaterial;
        if (hasVoidKey) VoidKeySlot.GetComponent<Renderer>().material = VoidKeyMaterial;
    }

    // Spawns remaining keys in world
    private void SpawnKeys()
    {
        if (!hasShadowKey) ShadowKeyObject.SetActive(true);
        if (!hasBloodKey) BloodKeyObject.SetActive(true);
        if (!hasVoidKey) VoidKeyObject.SetActive(true);
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
    }
}
