using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controls the behavior of doors that can be unlocked with keys.
/// </summary>

public class Door : MonoBehaviour
{
    [Header("----- Door -----")]
    [Tooltip("Key ID required to unlock this door.")]
    // Must match the ID of the key that opens this door
    [SerializeField] string requiredKeyId = "key1";

    [Header("----- Movement Settings -----")]
    [Tooltip("Speed at which the door opens.")]
    [SerializeField] float openSpeed = 1.0f;
    [Tooltip("Angle at which the door opens.")]
    [SerializeField] float openAngle = 90f;
    [SerializeField] Vector3 rotationAxis = Vector3.up;

    public bool isOpening = false;
    private bool isLocked = true;
    private Vector3 initialRotation;
    private float currentRotation = 0;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store the door's initial rotation so it can close it later
        initialRotation = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpening)
        {
            // Rotate the door
            currentRotation = Mathf.MoveTowards(currentRotation, openAngle, openSpeed * Time.deltaTime);

            // Rotate the door around the rotationAxis
            Vector3 newRotation = initialRotation;

            // Apply rotation based on the chosen axis
            if(rotationAxis == Vector3.up)
            {
                newRotation.y += currentRotation;
            }
            else if (rotationAxis == Vector3.right)
            {
                newRotation.x += currentRotation;
            }
            else if (rotationAxis == Vector3.forward)
            {
                newRotation.z += currentRotation;
            }

            // Apply the new rotation to the door
            transform.eulerAngles = newRotation;

            // Check if the door has finished opening
            if (currentRotation >= openAngle)
            {
                isOpening = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has the required key
        if (other.CompareTag("Player") && isLocked)
        {
            playerInventory inventory = other.GetComponent<playerInventory>();
            if (inventory != null && inventory.HasKey(requiredKeyId))
            {
                isLocked = false;
                isOpening = true;
                Debug.Log("Door unlocked with key: " + requiredKeyId);
            }
            else
            {
                Debug.Log("Door is locked. Requires key: " + requiredKeyId);
            }

        }
    }
}

