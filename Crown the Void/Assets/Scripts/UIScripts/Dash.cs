using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dash : MonoBehaviour
{
    public Slider DashBar;
    PlayerController pc;
    private float dashcooldown;

    // Start is called before the first frame update
    void Start()
    {
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        dashcooldown = pc.DODGE_CURRENT_COOLDOWN;
        UpdateDashCooldown(dashcooldown);

    }

    public void UpdateDashCooldown(float value)
    {
        DashBar.value = value;
    }
}
