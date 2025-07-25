using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionPopUpBehavior : MonoBehaviour
{
    private const float HIDE_TIMER_DURATION = 0.5f;
    private const float PLAYER_IN_VICINITY_TIMER = 0.001f;

    private Camera m_Camera;

    [SerializeField] private GameObject popUp;
    [SerializeField] private TextMeshProUGUI popUpText;

    private bool m_isHideTimerActive = false;
    private bool m_isDisplayed = false;
    private bool m_playerInVicinity = false;

    private void Start()
    {
        m_Camera = Camera.main;
    }

    private void LateUpdate()
    {
        Quaternion rotation = m_Camera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward,
            rotation * Vector3.up);
    }

    public void ShowPopUp(string promptText)
    {
        if (!m_isDisplayed)
        {
            popUpText.text = promptText;
            popUp.SetActive(true);
            m_isDisplayed = true;
        }

        if (!m_isHideTimerActive)
        {
            Invoke(nameof(HidePopUp), HIDE_TIMER_DURATION);
            m_isHideTimerActive = true;
        }

        m_playerInVicinity = true;
        Invoke(nameof(PlayerInVicinityScan), PLAYER_IN_VICINITY_TIMER);
    }

    private void PlayerInVicinityScan()
    {
        m_playerInVicinity = false;
    }

    private void HidePopUp()
    {
        if (!m_playerInVicinity)
        {
            popUp.SetActive(false);
            m_isDisplayed = false;
        }
        m_isHideTimerActive = false;
    }
}