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
    PlayerController pc;
    private int playerHealth;
    public Color DarkRed = new(176, 0, 0, 255);

    void Start()
    {
        pc = GetComponentInParent<PlayerController>();
        playerHealth = pc.playerHealth;
        TookDamage();
    }

    public void TookDamage()
    {
        playerHealth = pc.playerHealth;
        
        healthBar.value = playerHealth;
        healthText.text = playerHealth.ToString();
        if (playerHealth > 60)// like to change these to percentages based off max health
        {
            healthBar.fillRect.GetComponent<Image>().color = Color.green;
        }
        else if (playerHealth > 30)
        {
            healthBar.fillRect.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            healthBar.fillRect.GetComponent<Image>().color = DarkRed;
        }
    }
}

