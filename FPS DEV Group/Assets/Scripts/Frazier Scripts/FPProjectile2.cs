using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject impactVFX;
    public GameObject explosionPrefab;
    public float damage = 25f;
    public float explosionRadius = 5f;
    public bool explodeOnImpact = false;

    private bool collided;

    void OnCollisionEnter(Collision co)
    {
        if (co.gameObject.CompareTag("Bullet") || co.gameObject.CompareTag("Player") || collided)
            return;

        collided = true;

        EnemyHealth enemy = co.gameObject.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        if (impactVFX != null)
        {
            var impact = Instantiate(impactVFX, co.contacts[0].point, Quaternion.identity) as GameObject;

            Destroy(impact, 2);
        }

        if (explodeOnImpact)
        {
            Explode();
        }

        Destroy(gameObject);

    }

    void Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            EnemyHealth enemy = nearby.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

    }

}
