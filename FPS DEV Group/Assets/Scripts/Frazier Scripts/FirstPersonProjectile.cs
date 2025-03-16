using UnityEngine;

public class FirstPersonProjectile : MonoBehaviour
{
    public Camera cam;

    // Zooming

    public float zoomFOV = 30f;
    public float normalFOV = 60f;
    public float adsSpeed = 10f;
    private bool isAiming = false;

    public GameObject[] projectiles;
    public int selectedProjectile = 0;

    //public float adsSensitivity = 0.5f;
    //public float normalSensitivity = 1f;
    //private float currentSensitivity;

    public GameObject projectile;
    public GameObject muzzle;
    public float projectileSpeed = 30;
    public float fireRate = 4;
    public float arcRange = 1;


    private Vector3 destination;
    private float timeToFire;

    // Start is called before the first frame update
    void Start()
    {
        if (projectiles == null || projectiles.Length == 0)
        {
            Debug.LogError("No projectiles assigned to the array! Add projectile prefabs in the Inspector.");
        }

        //currentSensitivity = normalSensitivity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= timeToFire)
        {
            Debug.Log("Firing projectile...");
            timeToFire = Time.time + 1 / fireRate;
            ShootProjectile();
        }

        if (Input.GetMouseButton(1))
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }

        isAiming = Input.GetMouseButton(1);
        

        HandleZoom();

        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedProjectile = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && projectiles.Length > 1) selectedProjectile = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3) && projectiles.Length > 2) selectedProjectile = 2;

        if (selectedProjectile >= projectiles.Length)
        {
            Debug.LogError($"Selected Projectile Index ({selectedProjectile}) is out of bounds! Projectiles array size: {projectiles.Length}");
            selectedProjectile = 0;
        }
    }

    void HandleZoom()
    {
        float targetFOV = isAiming ? zoomFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, adsSpeed * Time.deltaTime);

    }

    void ShootProjectile()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        LayerMask weakPointLayer = LayerMask.GetMask("WeakPoint");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, weakPointLayer))
        {
            if (hit.collider.CompareTag("WeakPoint"))
            {
                destination = hit.point;

            }
            else
            {
                return;
            }

        }
        else
        {
            if (destination == Vector3.zero)
            {
                Debug.LogWarning("Dstination is invalid. Setting a fallback value.");
                destination = ray.GetPoint(1000);

            }
        }

        

        Transform firePoint = this.transform;
        InstantiateProjectile(firePoint);
    }

    void InstantiateProjectile(Transform firePoint)
    {
        

        if (projectiles == null || projectiles.Length == 0)
        {
            Debug.LogError("Projectiles array is empty! Assign projectile prefabs in the Inspector.");
            return;
        }

        if (selectedProjectile < 0 || selectedProjectile >= projectiles.Length)
        {
            Debug.LogError($"Selected projectile index ({selectedProjectile}) is out of bounds. Resetting to 0.");
            selectedProjectile = 0;
            return;
        }

        GameObject selectedProj = projectiles[selectedProjectile];
        var projectileObj = Instantiate(selectedProj, firePoint.position, Quaternion.identity) as GameObject;

        Debug.Log("Destination: " + destination);
        Vector3 direction = (destination - firePoint.position).normalized;
        Debug.Log("Projectile direction: " + direction);

        Rigidbody rb = projectileObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.angularVelocity = direction * projectileSpeed;
        }


        
    }

}
