using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviroCubeBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(2);
            Debug.Log("Gotcha!!");
        }
    }


    public void TakeDamage(int hitPoints)
    {
        Debug.Log("Ouch! You hit me for " +  hitPoints + " points!");
    }
}
