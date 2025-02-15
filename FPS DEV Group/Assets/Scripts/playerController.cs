using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class playerController : MonoBehaviour, IDamage
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

    [Header("----- Other Player Stats -----")]
    [SerializeField] int gravity;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    [Header("----- Player Melee Stats -----")]
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeRange;

    int jumpCount;
    int HPOrig;

    float shootTimer;
    
    Vector3 moveDir;
    Vector3 playerVel;
    bool isSprinting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        movement();
        sprint();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel =Vector3.zero;
        }
        //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //transform.position += moveDir * speed * Time.deltaTime; 

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + 
                  (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();

        controller.Move(playerVel * speed * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        shootTimer += Time.deltaTime;

        if(Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            shoot();
        }

    } 

    void sprint()
    {
        if(Input.GetButtonDown("Sprint")) 
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed * Time.deltaTime;

        }
    }

    void shoot()
    {
        shootTimer = 0;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);
        }
    }

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
        gamemanager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDamageScreen.SetActive(false);
    }

    void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }
}
