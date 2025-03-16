using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    public GameObject winText;
    public GameObject mainMenuButton;
    public GameObject GrayOut;
    private GameObject Player;
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player)
        {
            winText.SetActive(true);
            mainMenuButton.SetActive(true);
            GrayOut.SetActive(true);
            Time.timeScale = 0;
        }
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
        Destroy(Player);
    }
}
