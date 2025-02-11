using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controls the behavior of the keys that can unlock doors.
/// </summary>

public class Key : MonoBehaviour
{
    [Header("----- Key -----")]
    [Tooltip("Unique identifier for this key (needs to match a door's requiredKeyId).")]
    [SerializeField] string keyId = "key1";

    [Header("----- Visual Settings -----")]
    [Tooltip("Speed at which the key rotates.")]
    [SerializeField] float rotationSpeed = 50f;
    [Tooltip("Speed of the up/down bobbing motion.")]
    [SerializeField] float bobSpeed = 2f;
    [Tooltip("Height of the up/down bobbing motion.")]
    [SerializeField] float bobHeight = 0.5f;

    private Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Visual effects for key
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        
        float newY = startPosition.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            playerInventory inventory = other.GetComponent<playerInventory>();
            if (inventory != null)
            {
                inventory.AddKey(keyId);
                // Disable the key object after pick up
                gameObject.SetActive(false);
                
                // Destroy(gameObject);
            }
        }
    }

}
