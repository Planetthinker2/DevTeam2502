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

    [Header("----- Player Stamina -----")]
    [SerializeField] int maxStamina;
    [SerializeField] float currentStamina;
    [SerializeField] float staminaRegenRate = 10f;
    [SerializeField] float sprintStaminaCost = 15f;
    [SerializeField] int jumpStaminaCost;
    [SerializeField] float staminaRegenDelay;


    [Header("----- Player Gun Stats -----")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    [Header("----- Player Melee Stats -----")]
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeRange;


    int jumpCount;
    int HPOrig;
    int gunListPos;
    float shootTimer;
    float staminaRegenTimer;


    Vector3 moveDir;
    Vector3 playerVel;


    bool isShooting;
    bool isSprinting;
    bool canRegenStamina;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        currentStamina = maxStamina;
        canRegenStamina = true;
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red, 1.0f);
        movement();
        sprint();

        //shootTimer += Time.deltaTime;

   
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }
        //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //transform.position += moveDir * speed * Time.deltaTime; 

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + 
                  (Input.GetAxis("Vertical") * transform.forward);
        
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();

        //controller.Move(playerVel * speed * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;



        shootTimer += Time.deltaTime;

        if(Input.GetButton("Fire1") && shootTimer >= shootRate && !isShooting)
        {
            StartCoroutine(shoot());
        }

        selectGun();
        manageStamina();

    } 

    void manageStamina()
    {
        // When sprinting, reduce stamina
        if (isSprinting && moveDir.magnitude > 0.1f) // Only deplete when actually moving
        {
            // Calculate without casting to int
            currentStamina -= sprintStaminaCost * Time.deltaTime;
            canRegenStamina = false;
            staminaRegenTimer = 0;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                speed /= sprintMod;
                isSprinting = false;
            }
        }
        else
        {
            if (!canRegenStamina)
            {
                staminaRegenTimer += Time.deltaTime;
                if (staminaRegenTimer >= staminaRegenDelay)
                {
                    canRegenStamina = true;
                }
            }

            // Regenerate without casting to int
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

    void sprint()
    {
        if(Input.GetButtonDown("Sprint") && currentStamina > 0 && !isSprinting) 
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint") && isSprinting)
        {
            speed /= sprintMod;
            isSprinting = false;
            staminaRegenTimer = 0;
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax && currentStamina >= jumpStaminaCost)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;

            // Subtract the cost of jumping from the current stamina
            currentStamina -= jumpStaminaCost;
            canRegenStamina = false;
            staminaRegenTimer = 0;
            updatePlayerUI();

        }
    }

    IEnumerator shoot()
    {

        isShooting = true;
        shootTimer = 0;


        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    /*
    void meleeAttack()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if(dmg!= null)
            {
                dmg.takeDamage(meleeDamage);
            }
        }
    }
    

    public void getMeleeStats(meleeStats melee)
    {
        if(melee == null)
        {
            return;
        }
        meleeDamage = melee.damage;
        meleeRange = (int)melee.attackRate;
    }
    */

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashDamageScreen());
        updatePlayerUI();


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
        }
    }

    public void getGunStats(gunStats gun)
    {

        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
        changeGun();
       
    }

    void selectGun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count -1)
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
    }
}
