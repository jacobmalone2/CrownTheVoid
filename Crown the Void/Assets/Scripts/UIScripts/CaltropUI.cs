using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CaltropUI : MonoBehaviour
{
    private const int NUM_CALTROPS = 3;
    private const int CALTROP_COOLDOWN_TIME = 6;

    private int m_indxCaltropToUse;
    private int m_indxCaltropToRecharge;
    private int m_cooldownTime;

    [SerializeField] private GameObject[] m_caltropWidgets;
    [SerializeField] private GameObject[] m_caltropIcons;
    [SerializeField] private TextMeshProUGUI[] m_cooldownTimers;

    private void Start()
    {
        m_indxCaltropToRecharge = NUM_CALTROPS;
        m_indxCaltropToUse = NUM_CALTROPS - 1;
        m_cooldownTime = CALTROP_COOLDOWN_TIME;
    }

    // Updates cooldown timer
    public void SetCooldownTime(int time)
    {
        m_cooldownTime = time;
        m_cooldownTimers[m_indxCaltropToRecharge].text = m_cooldownTime.ToString();
    }

    // Updates UI when a caltrop is recharged
    public void GainCaltrop()
    {
        m_cooldownTime = CALTROP_COOLDOWN_TIME; // Reset the cooldown time for next recharge

        // If we have more caltrops to recharge, show the next widget and set its timer
        if (m_indxCaltropToRecharge < 2)
        {
            m_caltropWidgets[m_indxCaltropToRecharge + 1].SetActive(true);
            m_cooldownTimers[m_indxCaltropToRecharge + 1].text = m_cooldownTime.ToString();
        }

        // Show the icon of the recharged caltrop
        m_cooldownTimers[m_indxCaltropToRecharge].gameObject.SetActive(false);
        m_caltropIcons[m_indxCaltropToRecharge].SetActive(true);

        // Adjust indexes of recharging caltrop widget and widget for caltrop to use
        m_indxCaltropToUse = m_indxCaltropToRecharge;
        m_indxCaltropToRecharge++;
    }

    // Updates UI when a caltrop is dropped
    public void UseCaltrop()
    {
        // If we are already recharging a caltrop, hide that caltrop widget
        if (m_indxCaltropToRecharge <= 2)
            m_caltropWidgets[m_indxCaltropToRecharge].SetActive(false);

        // Hide icon of current caltrop to use and show cooldown timer
        m_caltropIcons[m_indxCaltropToUse].SetActive(false);
        m_cooldownTimers[m_indxCaltropToUse].gameObject.SetActive(true);

        // Set cooldown time to the timer value
        m_cooldownTimers[m_indxCaltropToUse].text = m_cooldownTime.ToString();

        // Adjust indexes of recharging caltrop widget and widget for caltrop to use
        m_indxCaltropToRecharge = m_indxCaltropToUse;
        m_indxCaltropToUse--;
    }
}
