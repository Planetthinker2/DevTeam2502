using UnityEngine;
using UnityEngine.UI;

public class FlameThrower : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_FlameThrower;
    public Camera playerCamera;
    public GameObject flameEffectPrefab;
    public Transform flamePoint;
    public Text ammoText;
    public float fireRate = 0.1f;
    public float maxAmmo = 360f;
    private float currentAmmo;
    private float lastFiredTime;
    

    private bool isFiring = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        m_FlameThrower.Stop();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            m_FlameThrower.Play();
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_FlameThrower.Stop();
        }

        Debug.Log("Current Ammo: " + currentAmmo);

        if (Input.GetButton("Fire1") && Time.time >= lastFiredTime + fireRate && currentAmmo > 0)
        {
            isFiring = true;
            FireFlamethrower();
        }
        else
        {
            isFiring = false;
        }

        if (isFiring)
        {
            currentAmmo -= 1f;
            UpdateAmmoUI();
        }

        if (currentAmmo < maxAmmo)
        {
            currentAmmo += 0.1f * Time.deltaTime;
            UpdateAmmoUI();
        }
    }

    void FireFlamethrower()
    {

        Debug.Log("Flamethrower Fired!");

        GameObject flameEffect = Instantiate(flameEffectPrefab, flamePoint.position, flamePoint.rotation);
        Destroy(flameEffect, 0.5f);

        lastFiredTime = Time.time;
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + Mathf.Ceil(currentAmmo).ToString() + " / " + maxAmmo.ToString();
        }
    }


}
