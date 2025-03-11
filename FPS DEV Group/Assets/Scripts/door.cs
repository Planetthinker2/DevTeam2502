using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Door script that can be opened and closed. Optionally locked with a key.
/// </summary>

public class door : MonoBehaviour
{
    [Header("----- Door Settings -----")]
    [SerializeField] Transform doorModel;
    [SerializeField] float openAngle = 90f;
    [SerializeField] float openSpeed = 2.0f;
    [SerializeField] bool startLocked;
    [SerializeField] string keyID; // ID that must match with a key to unlock
    [SerializeField] AudioClip doorSound; // Optional audio if wanted
    [SerializeField] float soundVolume = 0.5f; // Optional volume if wanted

    public bool isOpen = false;
    public bool isMoving = false;
    public bool isLocked;

    Quaternion closedRotation;
    Quaternion openRotation;
    float rotationTime = 0f;
    AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize door state
        isLocked = startLocked;

        // Get door rotation
        closedRotation = doorModel.rotation;
        openRotation = Quaternion.Euler(doorModel.eulerAngles + new Vector3(0, openAngle, 0));

        // Get or add audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && doorSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            if(isOpen)
            {
                // Opening animation
                rotationTime += Time.deltaTime * openSpeed;
                if(rotationTime >= 1.0f)
                {
                    rotationTime = 1.0f;
                    isMoving = false;
                }
            }
            else
            {
                // Closing animation
                rotationTime -= Time.deltaTime * openSpeed;
                if (rotationTime <= 0.0f)
                {
                    rotationTime = 0.0f;
                    isMoving = false;
                }
            }
            // Apply rotation
            doorModel.rotation = Quaternion.Slerp(closedRotation, openRotation, rotationTime);
        }
    }

    public void Interact()
    { 
        if(isLocked)
        {
            Debug.Log("The door is locked. A key could open it.");
            return;
        }

        // Toggle door state
        isOpen = !isOpen;
        isMoving = true;

        // Play sound effect
        if (audioSource != null && doorSound != null)
        {
            audioSource.PlayOneShot(doorSound, soundVolume);
        }
    }

    // Attempt to unlock the door with a key
    public bool UseKey(string keyIDToCheck)
    {
        if (isLocked && keyIDToCheck == keyID)
        {
            isLocked = false;
            Debug.Log("Door unlocked.");
            return true;
        }
        return false;
    }
}
