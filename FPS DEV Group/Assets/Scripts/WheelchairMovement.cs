using UnityEngine;

public class WheelchairMovement : MonoBehaviour
{
    public GameObject wheelchair;  
    public float moveSpeed = 2f;   
    public float moveDistance = 5f; 

    private bool isTriggered = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        if (wheelchair != null)
        {
            startPosition = wheelchair.transform.position;
            targetPosition = startPosition + wheelchair.transform.forward * moveDistance;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            isMoving = true;
        }
    }

    void Update()
    {
        if (isMoving && wheelchair != null)
        {
            // Move the entire wheelchair object, not just a child mesh
            wheelchair.transform.position = Vector3.MoveTowards(wheelchair.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(wheelchair.transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
    }
}
