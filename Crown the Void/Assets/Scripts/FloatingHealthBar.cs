using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Camera camera;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    // attempt 2
    //private Camera _cam;
    //void Start()
    //{
    //    _cam = Camera.main;
    //}

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
        if(currentValue <= 0)
        {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        // attempt 1
        //transform.rotation = camera.transform.rotation;
        // transform.position = target.position + offset;
        // attempt 2
        //transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position);

    }
}
