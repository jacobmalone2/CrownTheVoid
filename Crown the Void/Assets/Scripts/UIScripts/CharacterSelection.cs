using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private GameObject knight;
    [SerializeField] private GameObject ranger;
    public GameObject[] charactersPrefabs;
    public GameObject[] characters;
    public int selectedCharacter = 0;
    LoadCharacter LoadCharacter;

    public void NextCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter = (selectedCharacter + 1) % characters.Length;
        characters[selectedCharacter].SetActive(true);
    }

    public void PreviousCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter--;
        if (selectedCharacter < 0)
        {
            selectedCharacter += characters.Length;
        }
        characters[selectedCharacter].SetActive(true);
    }
    public void StartGame()
    {
        PlayerPrefs.SetInt("SelectedCharacter", selectedCharacter);
        SceneManager.LoadScene("StartingRoom");
        // Load the selected character prefab
        GameObject prefab = charactersPrefabs[selectedCharacter];
        GameObject player = Instantiate(prefab, new Vector3(0, 0.2f, 0), Quaternion.identity);
        player.GetComponent<KeyManager>().ClearKeyData();
        Destroy(knight);
        Destroy(ranger);
    }

}
