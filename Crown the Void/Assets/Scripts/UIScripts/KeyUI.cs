using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KeyUI : MonoBehaviour
{
    [SerializeField] private GameObject ShadowKeyIcon;
    [SerializeField] private GameObject BloodKeyIcon;
    [SerializeField] private GameObject VoidKeyIcon;

    [SerializeField] private KeyManager keyManager;

    public void UpdateKeyUI()
    {
        if (keyManager.HasShadowKey) ShadowKeyIcon.SetActive(true);
        if (keyManager.HasBloodKey) BloodKeyIcon.SetActive(true);
        if (keyManager.HasVoidKey) VoidKeyIcon.SetActive(true);
    }

    private void ClearKeyUI()
    {
        ShadowKeyIcon.SetActive(false);
        BloodKeyIcon.SetActive(false);
        VoidKeyIcon.SetActive(false);
    }
}
