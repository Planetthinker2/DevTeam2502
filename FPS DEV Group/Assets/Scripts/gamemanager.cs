using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TextMeshProUGUI enemyCountText;

    public Image playerHPBar;
    public GameObject playerDamageScreen;
    public bool isPaused;
    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPos;

    public int totalEnemies;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");

        // Initialize game state
        isPaused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive = null;

        // Initialize enemy count
        totalEnemies = 0;
        if (enemyCountText != null)
        {
            enemyCountText.text = "0";
        }
    }

    void Start()
    {
        // Count initial enemies
        CountEnemies();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel")) // Escape key
        {
            if (!isPaused)
            {
                menuActive = menuPause;
                statePause();
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    void CountEnemies()
    {
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        UpdateEnemyCountUI();
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        if (menuActive != null)
        {
            menuActive.SetActive(true);
        }
    }

    public void stateUnpause()
    {
        if (menuActive != null)
        {
            menuActive.SetActive(false);
        }

        isPaused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void UpdateEnemyCountUI()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = totalEnemies.ToString();
        }
    }

    public void updateGameGoal(int amount)
    {
        totalEnemies += amount;
        UpdateEnemyCountUI();

        if (totalEnemies <= 0)
        {
            menuActive = menuWin;
            statePause();
        }
    }

    public void youLose()
    {
        menuActive = menuLose;
        statePause();
    }
}