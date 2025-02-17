using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float damage = 25f;
    public float range = 50f;
    public Camera fpsCamera;
    public GameObject impactEffect;
    public AudioSource gunshotSound;

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // Left mouse click
        {
            Shoot();
        }
    }

    void Shoot()
    {
        gunshotSound.Play(); // Play gunshot sound

        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Spawn bullet impact effect
            GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 1f);
        }
    }
}
