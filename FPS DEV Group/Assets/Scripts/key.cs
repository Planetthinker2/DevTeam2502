using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Key script that governs the properties of the key game object.
/// </summary>

public class key : MonoBehaviour
{
    [Header("----- Key Settings -----")]
    [Tooltip("Unique identifier that must match a door's keyID to unlock it")]
    [SerializeField] string keyID;

    [Tooltip("Sound played when player picks up the key")]
    [SerializeField] AudioClip pickupSound;

    [Tooltip("Volume level for key pickup sound")]
    [SerializeField] float keySoundVolume = 0.5f;

    [Header("----- Visual Effects -----")]
    [Tooltip("How fast the key rotates")]
    [SerializeField] float rotationSpeed = 45f;

    [Tooltip("How high the key bobs up and down")]
    [SerializeField] float bobHeight = 0.2f;

    [Tooltip("How fast the key bobs up and down")]
    [SerializeField] float bobSpeed = 1.0f;

    private Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the key
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Make the key bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();

            if (player != null)
            {
                // Add key to player's inventory
                player.AddKey(keyID);

                // Play key pickup sound
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position, keySoundVolume);

                }
            }

            // Destroy the key in game world
            Destroy(gameObject);
        }
    }
}
