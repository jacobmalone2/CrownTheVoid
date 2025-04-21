using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadouts : MonoBehaviour
{
    [SerializeField] private GameObject[] characterLoadouts;


    public void ChangeCharacterLoadout(int selectedCharacter)
    {
        // Deactivate all loadout screens except the selected character
        for (int i = 0; i < characterLoadouts.Length; i++)
        {
            characterLoadouts[i].gameObject.SetActive(false);
        }
        characterLoadouts[selectedCharacter].gameObject.SetActive(true);
    }
}
