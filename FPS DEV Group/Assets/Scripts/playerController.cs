using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class playerController : MonoBehaviour
{

    [Header("----- Player Movement -----")]
    [Tooltip("The CharacterController component that handles player movement and collision.")]
    [SerializeField] CharacterController controller;

    [Tooltip("Speed at which the player moves (in units per second.)")]
    [SerializeField] int speed;

    [SerializeField] int sprintMod;

    [SerializeField] int jumpSpeed;

    [SerializeField] int jumpMax;

    [SerializeField] int gravity;

    int jumpCount;
    
    Vector3 moveDir;

    Vector3 playerVel;

    bool isSprinting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
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
}
