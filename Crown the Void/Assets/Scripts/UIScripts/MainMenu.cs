using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private bool hasShadowKey = false;
    private bool hasBloodKey = false;
    private bool hasVoidKey = false;

    [SerializeField] private Button continueButton;

    public GameObject playerObject;

    private void Start()
    {
        if (PlayerPrefs.HasKey("hasShadowKey"))
        {
            LoadKeyData();

            if (!hasShadowKey && !hasBloodKey && !hasVoidKey) continueButton.interactable = false;
        }
        else
        {
            continueButton.interactable = false;
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene("StartingRoom");
        GameObject player = Instantiate(playerObject, new Vector3(0, 0.2f, 0), Quaternion.identity);
        player.GetComponent<KeyManager>().ClearKeyData();
    }

    public void Continue()
    {
        SceneManager.LoadScene("StartingRoom");
        Instantiate(playerObject, new Vector3(0, 0.2f, 0), Quaternion.identity);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

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
