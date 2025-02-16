using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] Text goalCountText;

    public Image playerHPBar;
    public GameObject playerDamageScreen;
    public bool isPaused;
    public GameObject player;
    public playerController playerScript;

    int goalCount;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        // Initialize game state
        isPaused = false;
        menuActive = null;

        // Set initial cursor state
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Ensure time scale is set to 1
        Time.timeScale = 1;
    }

    void Start()
    {
        // Ensure all menu panels are initially disabled
        if (menuPause) menuPause.SetActive(false);
        if (menuWin) menuWin.SetActive(false);
        if (menuLose) menuLose.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel")) // Escape key
        {
            if (!isPaused)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    // Rest of your existing methods remain the same
    public void updateGameGoal(int amount)
    {
        goalCount += amount;
        goalCountText.text = goalCount.ToString("F0");
        if (goalCount <= 0)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}