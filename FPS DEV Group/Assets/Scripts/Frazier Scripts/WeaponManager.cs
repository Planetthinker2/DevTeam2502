using UnityEngine;

public class WeaponManager : MonoBehaviour
{

    public GameObject currentWeapon;
    public GameObject[] weapons;
    public Transform weaponParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(weapons.Length > 0)
        {
            EquipWeapon(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipWeapon(2);
        }
    }
    
    void EquipWeapon(int weaponIndex)
    {
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }

        if (weaponIndex >= 0 && weaponIndex < weapons.Length)
        {
            currentWeapon = weapons[weaponIndex];
            currentWeapon.SetActive(true);

            currentWeapon.transform.SetParent(weaponParent);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
        }
    }

}
