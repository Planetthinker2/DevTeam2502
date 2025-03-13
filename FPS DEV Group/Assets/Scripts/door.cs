using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Door script that allows doors be opened and closed. Optionally locked with a key.
/// </summary>

public class door : MonoBehaviour
{
    [Header("----- Door Settings -----")]
    [Tooltip("The door mesh or object that will rotate when opening/closing")]
    [SerializeField] Transform doorModel;

    [Tooltip("How far the door opens in degrees")]
    [SerializeField] float openAngle = 90f;

    [Tooltip("How fast the door opens and closes (higher = faster)")]
    [SerializeField] float openSpeed = 2.0f;

    [Tooltip("If checked, door will be locked and will require a key to open")]
    [SerializeField] bool startLocked;

    [Tooltip("Unique identifier that must match a key's ID to unlock this door")]
    [SerializeField] string keyID;

    [Tooltip("Sound played when door opens or closes")]
    [SerializeField] AudioClip doorSound;

    [Tooltip("Volume level for door sounds")]
    [SerializeField] float soundVolume = 0.5f;


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
