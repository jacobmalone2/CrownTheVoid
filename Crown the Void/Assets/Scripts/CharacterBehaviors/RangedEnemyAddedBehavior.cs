using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class RangedEnemyAddedBehavior : MonoBehaviour
{
    private EnemyBehavior eB;
    private Animator enemyAnimator;

    [SerializeField] private GameObject projectile;

    [SerializeField] private Transform ShootPoint;
    [SerializeField] private float shootDelay = 3f;

    [SerializeField] private float launchForce = 50f;
    [SerializeField] private float destroyTimer = 50f;

    private bool canShoot = true;

    // Start is called before the first frame update
    void Start()
    {
        eB = GetComponent<EnemyBehavior>();
        enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (eB.isAlive)
        {
            if (eB.playerInSightRange || enemyAnimator.GetBool("isShooting")) ShootPlayer();
        }
    }

    public void ShootPlayer()
    {
        eB.agent.isStopped = true;
        enemyAnimator.SetBool("isShooting", true);
        eB.playerInSightRange = true;

        // Get the target position to look at
        Vector3 targetPosition = new Vector3(eB.player.transform.position.x, eB.player.transform.position.y, eB.player.transform.position.z);

        // Calculate the "LookAt" rotation
        Quaternion lookRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);

        // Extract only the Y rotation 
        float yRotation = lookRotation.eulerAngles.y;

        // Apply only the Y rotation to the object
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public IEnumerator ShootDelay(float shootDelay)
    {
        yield return new WaitForSeconds(shootDelay);

        GameObject projectileShot = Instantiate(projectile, ShootPoint.position, Quaternion.Euler(-90, transform.rotation.eulerAngles.y, 0));

        // Apply the direction
        Rigidbody projectileRigidbody = projectileShot.GetComponent<Rigidbody>();
        if (projectileRigidbody != null)
        {
            projectileRigidbody.velocity = transform.forward * launchForce;
            Destroy(projectileShot, destroyTimer);
        }
        eB.ResetAnimator();
        eB.agent.isStopped = false;
    }
}
