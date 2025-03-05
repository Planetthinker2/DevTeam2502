using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerController : MonoBehaviour, IDamage, IPickup
{
    [Header("----- Player Controller -----")]
    [Tooltip("The CharacterController component that handles player movement and collision.")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("----- Main Player Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] Transform muzzleFlash;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    [Header("----- Player Stamina -----")]
    [SerializeField] int maxStamina;
    [SerializeField] float staminaRegenRate = 10f;
    [SerializeField] float sprintStaminaCost = 15f;
    [SerializeField] int jumpStaminaCost;
    [SerializeField] float staminaRegenDelay;

    [Header("----- Player Melee Stats -----")]
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeRange;


    [Header("--------Audio-----------")]
    [SerializeField] AudioSource aud;

    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audJump;
    [SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audWalk;
    [SerializeField] float audWalkVol;

    [Header("----- Collision Settings -----")]
    [SerializeField] float collisionPushForce = 2.0f;

    [Header("----- Flashlight Settings -----")]
    [SerializeField] GameObject flashlightObject;  // Parent object containing light and quad
    [SerializeField] Light flashlightLight;        // Light component
    [SerializeField] GameObject flashlightBeam;    // Quad for visual beam effect
    [SerializeField] float flashlightIntensity = 2.5f;
    [SerializeField] float flashlightRange = 10f;
    [SerializeField] float flashlightAngle = 45f;
    [SerializeField] KeyCode flashlightToggleKey = KeyCode.F;
    [SerializeField] float flashlightBatteryMax = 100f;
    [SerializeField] float flashlightBatteryDrain = 5f;  // Per second
    [SerializeField] float flashlightBatteryRecharge = 7.5f; // Per second

    int jumpCount;
    int HPOrig;
    int gunListPos;
    float shootTimer;
    float currentStamina;
    float staminaRegenTimer;
    private bool flashlightActive = false;
    private float flashlightBattery;

    Vector3 moveDir;
    Vector3 playerVel;

    bool isShooting;
    bool isSprinting;
    bool isPlayingSteps;
    bool canRegenStamina;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        currentStamina = maxStamina;
        canRegenStamina = true;
        flashlightBattery = flashlightBatteryMax;
        updatePlayerUI();

        // Initialize flashlight in on state
        flashlightActive = true;
        if (flashlightObject != null)
        {
            flashlightObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        movement();
        sprint();
        shootTimer += Time.deltaTime;
        manageStamina();
        manageFlashlight();

        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer >= shootRate && !isShooting)
        {
            StartCoroutine(shoot());
            gunList[gunListPos].ammoCur--;
            updateAmmoUI();
        }
        if (Input.GetButton("Reload"))
        {
            gunReload();
        }
        selectGun();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            if (moveDir.magnitude > 0.1f && !isPlayingSteps)
            {
                StartCoroutine(playWalk());
            }
            jumpCount = 0;
            playerVel.y = 0;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();

        controller.Move(playerVel * speed * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        // Add wall collision detection to prevent clipping
        if (moveDir.magnitude > 0.1f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, moveDir, out hit, controller.radius + 0.2f))
            {
                // Adjust movement to slide along walls
                Vector3 reflectDir = Vector3.Reflect(moveDir, hit.normal);
                moveDir = Vector3.Lerp(moveDir, reflectDir.normalized, 0.9f);
            }
        }
    }

    void manageStamina()
    {
        // Reduce stamina when player is sprinting
        if (isSprinting && moveDir.magnitude > 0.1f) // Only deplete when actually moving
        {
            // Subtract the cost of sprinting from the current stamina (per second)
            currentStamina -= sprintStaminaCost * Time.deltaTime;
            canRegenStamina = false;
            staminaRegenTimer = 0;

            // If out of stamina, stop sprinting
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                speed /= sprintMod;
                isSprinting = false;
            }
        }
        else
        {
            // Only regenerate after a delay
            if (!canRegenStamina)
            {
                staminaRegenTimer += Time.deltaTime;
                if (staminaRegenTimer >= staminaRegenDelay)
                {
                    canRegenStamina = true;
                }
            }

            // Regenerate stamina
            if (canRegenStamina && currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;

                if (currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }
            }
        }

        updatePlayerUI();
    }

    void manageFlashlight()
    {
        // Toggle flashlight on/off
        if (Input.GetKeyDown(flashlightToggleKey) && flashlightBattery > 0)
        {
            flashlightActive = !flashlightActive;
            flashlightObject.SetActive(flashlightActive);
        }

        // Handle battery drain and recharge
        if (flashlightActive)
        {
            flashlightBattery -= flashlightBatteryDrain * Time.deltaTime;

            if (flashlightBattery <= 0)
            {
                flashlightBattery = 0;
                flashlightActive = false;
                flashlightObject.SetActive(false);
            }
        }
        else
        {
            flashlightBattery += flashlightBatteryRecharge * Time.deltaTime;
            flashlightBattery = Mathf.Min(flashlightBattery, flashlightBatteryMax);
        }

        // Update UI (you can add a flashlight battery UI element)
        updatePlayerUI();
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint") && currentStamina > 0 && !isSprinting && moveDir.magnitude > 0.1f)
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if ((Input.GetButtonUp("Sprint") || currentStamina <= 0 || moveDir.magnitude < 0.1f) && isSprinting)
        {
            speed /= sprintMod;
            isSprinting = false;
            staminaRegenTimer = 0;
        }
    }

    IEnumerator playWalk()
    {
        isPlayingSteps = true;
        aud.PlayOneShot(audWalk[Random.Range(0, audWalk.Length)], audWalkVol);

        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.2f);

        isPlayingSteps = false;
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax && currentStamina >= jumpStaminaCost)
        {
            // Increment jump count
            jumpCount++;

            // Apply jump force
            playerVel.y = jumpSpeed;

            // Play Jump sound
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);

            // Subtract the cost of jumping from the current stamina
            currentStamina -= jumpStaminaCost;
            canRegenStamina = false;
            staminaRegenTimer = 0;

            // Update UI after stamina change
            updatePlayerUI();
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;
        shootTimer = 0;
        aud.PlayOneShot(gunList[gunListPos].shootSound[Random.Range(0, gunList[gunListPos].shootSound.Length)], gunList[gunListPos].shootVol);
        StartCoroutine(flashMuzzle());

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if (amount < 0)
        {
            StartCoroutine(flashRestoreScreen());
        }
        else if (amount > 0)
        {
            StartCoroutine(flashDamageScreen());
        }

        updatePlayerUI();

        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        if (HP > HPOrig)
        {
            HP = HPOrig;
        }

        if (HP <= 0)
        {
            gamemanager.instance.youLose();
        }
    }

    IEnumerator flashDamageScreen()
    {
        if (gamemanager.instance != null && gamemanager.instance.playerDamageScreen != null)
        {
            gamemanager.instance.playerDamageScreen.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            if (gamemanager.instance != null && gamemanager.instance.playerDamageScreen != null)
            {
                gamemanager.instance.playerDamageScreen.SetActive(false);
            }
        }
        else { yield return null; }
    }

    IEnumerator flashRestoreScreen()
    {
        if (gamemanager.instance != null && gamemanager.instance.playerRestoreScreen != null)
        {
            gamemanager.instance.playerRestoreScreen.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            if (gamemanager.instance != null && gamemanager.instance.playerRestoreScreen != null)
            {
                gamemanager.instance.playerRestoreScreen.SetActive(false);
            }
        }
        else { yield return null; }
    }

    void updatePlayerUI()
    {
        if (gamemanager.instance != null)
        {
            if (gamemanager.instance.playerHPBar != null)
            {
                gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
            }

            if (gamemanager.instance.playerStaminaBar != null)
            {
                gamemanager.instance.playerStaminaBar.fillAmount = currentStamina / maxStamina;
            }

            // Add UI for flashlight battery if needed
            // if (gamemanager.instance.flashlightBatteryBar != null)
            // {
            //     gamemanager.instance.flashlightBatteryBar.fillAmount = flashlightBattery / flashlightBatteryMax;
            // }
        }
    }

    void updateAmmoUI()
    {
        gamemanager.instance.updateAmmoUI(gunList[gunListPos].ammoCur, gunList[gunListPos].ammoMax);
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
        changeGun();
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
    }

    void changeGun()
    {
        shootDamage = gunList[gunListPos].shootDamage;
        shootRate = gunList[gunListPos].shootRate;
        shootDist = gunList[gunListPos].shootDist;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
        updateAmmoUI();
    }

    void gunReload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
        }
        gamemanager.instance.updateAmmoUI(gunList[gunListPos].ammoCur, gunList[gunListPos].ammoMax);
    }

    IEnumerator flashMuzzle()
    {
        muzzleFlash.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        muzzleFlash.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.gameObject.SetActive(false);
    }
}