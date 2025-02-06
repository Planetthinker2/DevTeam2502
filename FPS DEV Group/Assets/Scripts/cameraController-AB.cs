using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Header("- - - - - - - - - Camera Setting Variables - - - - - - - - -")]

    [SerializeField] int sens;
    [Tooltip("Minimum and maximum the camera can look up and down - respective")]
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;
    [SerializeField] bool invertY;
    Vector3 lookDir;

    float rotX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //get input 
        lookDir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Depth"), Input.GetAxis("Vertical"));

        // tie the mouseY to rotate the cameras X

        // clamp the camera on the X-axis 

        // rotate the camera on the X-axis

        // rotate the player on the y-axis -- look left & right 
    }
}
