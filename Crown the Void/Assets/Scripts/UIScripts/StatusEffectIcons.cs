using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectIcons : MonoBehaviour
{
    [SerializeField] private Slider m_attackUpGauge;
    [SerializeField] private Slider m_defenceUpGauge;

    public void ShowAttackUp()
    {
        m_attackUpGauge.gameObject.SetActive(true);
        m_attackUpGauge.value = 1f;
    }

    public void ShowDefenceUp()
    {
        m_defenceUpGauge.gameObject.SetActive(true);
        m_defenceUpGauge.value = 1f;
    }

    public void UpdateAttackUp(float value)
    {
        m_attackUpGauge.value = value;
    }

    public void UpdateDefenceUp(float value)
    {
        m_defenceUpGauge.value = value;
    }

    public void HideAttackUp()
    {
        m_attackUpGauge.gameObject.SetActive(false);
    }

    public void HideDefenceUp()
    {
        m_defenceUpGauge.gameObject.SetActive(false);
    }
}
