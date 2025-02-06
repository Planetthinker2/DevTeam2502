using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    [Header("- - - - - - - - Player Setting Variables - - - - - - - -")]
    [SerializeField] CharacterController controller;
    [SerializeField] int speed = 1;
    [SerializeField] bool canRun = true;

    Vector3 moveDir; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();   
    }
    void Movement()
    {

        moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(moveDir * speed * Time.deltaTime);
        
        /*if(canRun && Input.GetButtonDown("LeftShift"))  
        else
        {
        controller.Move(moveDir * speed * Time.deltaTime);

        }*/

    }

}
