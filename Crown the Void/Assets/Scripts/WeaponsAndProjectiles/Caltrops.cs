using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// "Caltrops" (https://skfb.ly/oDFwO) by jorge_roku
// is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
public class Caltrops : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyBehavior>().Stun();
            Destroy(gameObject);
        }
    }
}
