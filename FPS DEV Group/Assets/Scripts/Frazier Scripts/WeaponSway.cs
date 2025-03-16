using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float swayAmount = 0.5f;
    public float maxSwayamount = 1.5f;
    public float smoothAmount = 6f;

    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.localRotation;
    }

    // Update is called once per frame
    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * swayAmount;


        mouseX = Mathf.Clamp(mouseX, -maxSwayamount, maxSwayamount);
        mouseY = Mathf.Clamp(mouseY, -maxSwayamount, maxSwayamount);

        Quaternion targetRotationX = Quaternion.AngleAxis(-mouseX, Vector3.up);
        Quaternion targetRotationY = Quaternion.AngleAxis(mouseY, Vector3.right);
        Quaternion targetRotation = initialRotation * targetRotationX * targetRotationY;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smoothAmount);

    }
}

