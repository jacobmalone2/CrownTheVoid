using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    public TextMeshProUGUI healthText;
    PlayerHealth playerHealth;
    public Color DarkRed = new(176, 0, 0, 255);

    void Start()
    {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        
    }
    void Update()
    {
        healthBar.value = playerHealth.health;
        healthText.text = playerHealth.health.ToString();
        if (playerHealth.health > 60)// like to change these to percentages based off max health
        {
            healthBar.fillRect.GetComponent<Image>().color = Color.green;
        }
        else if (playerHealth.health > 30)
        {
            healthBar.fillRect.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            healthBar.fillRect.GetComponent<Image>().color = DarkRed;
        }
    }
}

