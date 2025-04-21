using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    public GameObject[] charactersPrefabs;

    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter");
        GameObject prefab = charactersPrefabs[selectedCharacter];
        GameObject clone = Instantiate(prefab, new Vector3(0, 0.2f, 0), Quaternion.identity);

    }

}
