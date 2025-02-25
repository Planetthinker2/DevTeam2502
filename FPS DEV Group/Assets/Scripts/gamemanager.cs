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

    public int totalEnemies;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
    }

   

    void Update()
    {
        if (Input.GetButtonDown("Cancel")) // Escape key
        {
            if (menuActive == null)
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
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {

        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }


    public void updateGameGoal(int amount)
    {
        totalEnemies += amount;
        enemyCountText.text = totalEnemies.ToString("F0");
        //UpdateEnemyCountUI();

        if (totalEnemies <= 0)
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