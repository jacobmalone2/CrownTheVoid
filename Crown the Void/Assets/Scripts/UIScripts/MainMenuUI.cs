using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject menuScreen;

    public void ShowCredits()
    {
        menuScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    public void ShowSettings()
    {
        menuScreen.SetActive(false);
        settingsScreen.SetActive(true);
    }

    public void BackToMenu()
    {
        creditsScreen.SetActive(false);
        settingsScreen.SetActive(false);
        menuScreen.SetActive(true);
    }
}
