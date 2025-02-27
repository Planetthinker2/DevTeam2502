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
    [SerializeField] GameObject ammoDisplay;
    [SerializeField] TextMeshProUGUI enemyCountText;
    [SerializeField] TMP_Text ammoText;

    public Image playerHPBar;
    public Image playerStaminaBar;
    public GameObject playerDamageScreen;
    public GameObject playerRestoreScreen;
    public bool isPaused;
    public GameObject player;
    public playerController playerScript;

    public int totalEnemies;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        menuActive = null;
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

    public void updateGameGoal(int amount)
    {
        totalEnemies += amount;

        if (enemyCountText != null)
        {
            enemyCountText.text = totalEnemies.ToString("F0");
        }

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

    public void updateAmmoUI(int current, int max)
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo ";
            ammoText.text += current.ToString("F0");
            ammoText.text += " / ";
            ammoText.text += max.ToString("F0");
        }
    }
}