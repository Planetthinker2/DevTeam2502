using Unity.VisualScripting;
using UnityEngine;
using VolumetricLines;

public class LaserWeapon : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject impactPrefab;
    public GameObject explosionPrefab;
    public GameObject muzzleFlashPrefab;
    public Transform firePoint;
    public float laserRange = 100f;
    public float laserDamage = 25f;
    public float laserWidth = 0.1f;
    public float laserDuration = 0.05f;
    public LayerMask enemyLayer;
    public float explosionRadius = 5f;
    public float explosionForce = 10f;

    private VolumetricLineBehavior laserLine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        laserLine = gameObject.AddComponent<VolumetricLineBehavior>();
        laserLine.LineWidth = laserWidth;
        laserLine.LineColor = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            FireLaser();
        }
    }

    void FireLaser()
    {
        PlayMuzzleFlash();

        RaycastHit hit;
        Vector3 laserEndPoint = firePoint.position + firePoint.forward * laserRange;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, laserRange, enemyLayer))
        {
            laserEndPoint = hit.point;

            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if(enemy != null)
            {
                enemy.TakeDamage(laserDamage);
            }
            Debug.Log("Laser hit: " + hit.collider.name);
        } else
        {
            Debug.Log("Laser missed");
        }
        if(impactPrefab != null)
        {
            Instantiate(impactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
        }

        if(explosionPrefab != null)
        {
            TriggerExplosion(hit.point);
        }

        DrawLaser(firePoint.position, laserEndPoint);

        Invoke("ClearLaser", laserDuration);
    }


    void PlayMuzzleFlash()
    {
        if (muzzleFlashPrefab != null)
        {
            Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
        }
    }

    void DrawLaser(Vector3 start, Vector3 end)
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);


    }

    void ClearLaser()
    {
        LineRenderer lineRenderer = laserLine.GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position);
        }
    }

    void TriggerExplosion(Vector3 explosionPoint)
    {
        Instantiate(explosionPrefab, explosionPoint, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(explosionPoint, explosionRadius);
        foreach (var collider in colliders)
        {
            if(collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddExplosionForce(explosionForce, explosionPoint, explosionRadius);

                if(collider.TryGetComponent<EnemyHealth>(out EnemyHealth enemy))
                {
                    enemy.TakeDamage(laserDamage);
                }
            }
        }
    }
}
