using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBomb : MonoBehaviour
{
    private ActiveBomb bomb;

    private void Start()
    {
        bomb = GetComponentInParent<ActiveBomb>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            bomb.Explode();
        }
    }
}
