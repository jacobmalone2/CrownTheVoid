using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject controlsScreen;

    public void ShowSettings()
    {
        settingsScreen.SetActive(true);
        menuScreen.SetActive(false);
    }

    public void ShowControls()
    {
        controlsScreen.SetActive(true);
        menuScreen.SetActive(false);
    }

    public void BackToMenu()
    {
        settingsScreen.SetActive(false);
        controlsScreen.SetActive(false);
        menuScreen.SetActive(true);
    }
}
