using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the player's inventory of keys and etc.
/// </summary>

public class playerInventory : MonoBehaviour
{
    [Header("----- Player Inventory -----")]
    [Tooltip("Max number of keys the player can collect.")]
    [SerializeField] private int maxKeys = 10;

    // Array to store the keys the player has collected
    private string[] keyInventory;
    private int currentKeyCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the key inventory array
        keyInventory = new string[maxKeys];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool AddKey(string keyId)
    {
        // Check if the player has already collected this key
        if (currentKeyCount < maxKeys)
        {
            keyInventory[currentKeyCount] = keyId;
            currentKeyCount++;
            Debug.Log($"Collected key: {keyId}. Total Keys: {currentKeyCount}" );
            return true;
        }
        else
        {
            Debug.Log("Inventory is full!");
            return false;
        }
    }

    public bool HasKey(string keyId)
    {
        for (int i = 0; i < currentKeyCount; i++)
        {
            if (keyInventory[i] == keyId)
            {
                return true;
            }
        }
        return false;
    }

    public bool RemoveKey(string keyId)
    {
        for (int i = 0; i < currentKeyCount; i++)
        { 
            if(keyInventory[i] == keyId)
            {
                // Shift keys down to fill the gap
                for(int j = i; j < currentKeyCount - 1; j++)
                {
                    keyInventory[j] = keyInventory[j + 1];
                }
                currentKeyCount--;
                keyInventory[currentKeyCount] = null;
                Debug.Log($"Used key: {keyId}. Total Keys: {currentKeyCount}");
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets the current number of keys the player has collected.
    /// </summary>

    public int GetKeyCount()
    {
        return currentKeyCount;
    }

    /// <summary>
    /// Gets the maximum number of keys the player can collect.
    /// </summary>
    public int GetMaxKeys()
    {
        return maxKeys;
    }

    /// <summary>
    /// Gets a specific key at the given index.
    /// </summary>
    public string GetKeyAtIndex(int index)
    {
        if (index >= 0 && index < currentKeyCount)
        {
            return keyInventory[index];
        }
        return null;
    }

    public void PrintInventory()
    {
        string inventoryContents = "Inventory Contents: ";
        for (int i = 0; i < currentKeyCount; i++)
        {
            inventoryContents += keyInventory[i] + " ";
        }
        Debug.Log(inventoryContents);
    }

}
