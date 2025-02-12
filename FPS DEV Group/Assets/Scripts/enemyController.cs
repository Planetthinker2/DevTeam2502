using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// This class handles the movement and behavior of the enemy.
/// </summary>

public class enemyController : MonoBehaviour
{
    [Header("----- Enemy Movement Settings -----")]
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float runThreshold = 10f; // Distance at which enemy starts running
    [SerializeField] float attackRange = 2f;

    [Header("----- Combat Settings -----")]
    [SerializeField] int meleeDamage = 20;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] float attackAnimationDuration = 0.5f;

    private Transform player;
    private NavMeshAgent agent;
    private float nextAttackTime;
    private bool isAttacking;
    private Animator animator; // Reference to enemy's animator component


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        agent.stoppingDistance = attackRange;
        agent.speed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        UpdateMovementBehavior(distanceToPlayer);
        
        if (CanAttack(distanceToPlayer))
        {
            StartCoroutine(PerformAttack());
        }
    }

    void UpdateMovementBehavior(float distanceToPlayer)
    {
        // Don't move if attacking
        if (isAttacking)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        // Set destination to player's position
        agent.SetDestination(player.position);
        // Adjust speed based on distance to player
        if (distanceToPlayer > runThreshold)
        {
            agent.speed = runSpeed;
            if (animator != null) animator.SetBool("isRunning", true);
        }
        else
        {
            agent.speed = walkSpeed;
            if (animator != null) animator.SetBool("isRunning", false);
        }
    }

    bool CanAttack(float distanceToPlayer)
    {
        return !isAttacking && distanceToPlayer 
            <= attackRange && Time.time >= nextAttackTime;
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        // Play attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

        /*
        // Deal damage if still in range
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(meleeDamage);
            }
        }
        */

        // Wait for the rest of the animation
        yield return new WaitForSeconds(attackAnimationDuration * 0.5f);
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, runThreshold);
    }
}

