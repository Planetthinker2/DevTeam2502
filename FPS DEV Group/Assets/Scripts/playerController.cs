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
    Vector3 moveDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
     movement();
    }

    void movement()
    {
        //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //transform.position += moveDir * speed * Time.deltaTime; 

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + 
                  (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);
    } 
}
