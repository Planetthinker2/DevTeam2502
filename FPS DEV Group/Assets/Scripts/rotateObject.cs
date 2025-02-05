using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateObject : MonoBehaviour
{
    [Header("----- Rotation -----")]
    [Tooltip("Speed at which the object rotates around the Y-axis (in degrees per second).")]
    [SerializeField] int speed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * speed); // Can increase speed by multiplying by a number
    }
}

