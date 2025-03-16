using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
        if (currentValue <= 0)
        {
            Destroy(gameObject);
        }
    }

    void LateUpdate()
    {
        // Set the rotation to face a fixed direction
        transform.rotation = Quaternion.Euler(0, 0, 0); // Adjust the values as needed
    }
}
