using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpinneyAxe : MonoBehaviour
{
    [SerializeField] GameObject SpinneyAxeVisuals;
    [SerializeField] float rotationSpeed = -500f;
    private PlayerController pc;
    Rigidbody rb;
    Vector3 velocity;

    void Start()
    {
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.forward * -7.0f, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {
        velocity = rb.velocity;
        SpinneyAxeVisuals.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("The Player Dies");
            pc.TakeDamage(100);
            this.gameObject.SetActive(false);
        }

        if (other.collider.CompareTag("Wall") || other.collider.CompareTag("Boss"))
        {
            float speed = velocity.magnitude;

            // Reflect params must be normalized so we get new direction
            Vector3 direction = Vector3.Reflect(velocity.normalized, other.contacts[0].normal);
            rb.AddForce(Vector3.forward * -2.0f, ForceMode.VelocityChange);

            // Like earlier wrote: velocity vector is magnitude (speed) and direction (a new one)
            rb.velocity = direction * speed;
        }
    }
}
