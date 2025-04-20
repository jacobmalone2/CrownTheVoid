using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject menuScreen;

    public void ShowCredits()
    {
        menuScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    public void BackToMenu()
    {
        creditsScreen.SetActive(false);
        menuScreen.SetActive(true);
    }
}
