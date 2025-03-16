using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using TMPro.EditorUtilities;

public class EnemyHealth : MonoBehaviour
{

    public float maxHealth = 500f;
    private float currentHealth;
    public Slider healthBar;
    public GameObject deathEffect;
    public string nextSceneName = "NextStage";

    [Header("Health Bar UI")]
    public GameObject healthBarPrefab;
    private GameObject healthbarInstance;
    private Slider healthSlider;
    private TextMeshProUGUI nameText;
    private Transform uiTransform;

    public string enemyName = "Boss";
    private Animator animator;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            healthbarInstance = Instantiate(healthBarPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
            healthSlider = healthbarInstance.GetComponentInChildren<Slider>();
            nameText = healthbarInstance.GetComponentInChildren<TextMeshProUGUI>();
            uiTransform = healthbarInstance.transform;

            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            nameText.text = enemyName;

        }
    }

    void Update()
    {
        if (healthbarInstance != null)
        {
            uiTransform.position = transform.position + Vector3.up * 2;
            uiTransform.LookAt(Camera.main.transform);

        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(enemyName + "took" + damage + " damage! Remaining Health: " + currentHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;

        }

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        if (currentHealth <= 0)
        {

            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Debug.Log(enemyName + "Boss Defeated! You win!");
        //StartCoroutine(WinGame());

        if (healthbarInstance != null)
        {
            Destroy(healthbarInstance);
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        Debug.Log("Boss Defeated!");
        Destroy(gameObject, 2f);
    }

    
}

