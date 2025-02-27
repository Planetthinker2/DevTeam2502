using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyController : MonoBehaviour, IDamage
{
    [Header("----- Enemy Settings -----")]
    [SerializeField] Renderer model;
    [SerializeField] Transform player;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] int HP;
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float detectionRange = 10f;
    [SerializeField] GameObject loot;

    [Header("----- Patrol Settings -----")]
    [SerializeField] float patrolRadius = 10f;
    [SerializeField] float minPatrolWaitTime = 1f;
    [SerializeField] float maxPatrolWaitTime = 3f;
    [SerializeField] float minPatrolMoveTime = 1f;
    [SerializeField] float angleFOV = 90f;

    [Header("----- Combat Settings -----")]
    [SerializeField] int meleeDamage = 20;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] float attackAnimationDuration = 0.5f;

    bool hasDetectedPlayer = false;
    float nextAttackTime;
    bool isAttacking;
    Vector3 startPosition;
    Vector3 patrolDestination;
    bool patrolWaiting;
    float patrolWaitTimer;

    Color colorOrig;
    Coroutine co;

    private readonly string ANIM_IS_WALKING = "isWalking";
    private readonly string ANIM_IS_RUNNING = "isRunning";
    private readonly string ANIM_ATTACK = "Attack";
    private readonly string ANIM_SPEED = "Speed";

    void Start()
    {
        colorOrig = model.material.color;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        gameObject.tag = "Enemy";

        agent.stoppingDistance = attackRange;
        agent.speed = walkSpeed;

        // Store starting position for patrol system
        startPosition = transform.position;
        SetNewPatrolDestination();

        SetAnimationState(1);

        /*if (gamemanager.instance != null)
        {
            gamemanager.instance.updateGameGoal(1);
        }*/
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if player is in FOV and should be detected
        if (!hasDetectedPlayer)
        {
            hasDetectedPlayer = IsPlayerInFOV();
        }

        // Choose behavior based on detection state
        if (hasDetectedPlayer)
        {
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            Patrol();
        }
    }

    bool IsPlayerInFOV()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        // Check distance and angle
        if (Vector3.Distance(transform.position, player.position) <= detectionRange && angle <= angleFOV * 0.5f)
        {
            // Check line of sight with raycast
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer.normalized, out hit, detectionRange))
            {
                if (hit.transform == player)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void Patrol()
    {
        // Wait at destination for random time
        if (patrolWaiting)
        {
            patrolWaitTimer += Time.deltaTime;
            if (patrolWaitTimer >= Random.Range(minPatrolWaitTime, maxPatrolWaitTime))
            {
                patrolWaiting = false;
                SetNewPatrolDestination();
            }
            SetAnimationState(0); // Idle while waiting
            return;
        }

        // Check if destination reached
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolWaiting = true;
            patrolWaitTimer = 0f;
            return;
        }

        // Normal patrol walking
        agent.speed = walkSpeed;
        SetAnimationState(1);
    }

    void SetNewPatrolDestination()
    {
        // Find random point on navmesh within radius
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startPosition;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            patrolDestination = hit.position;
            agent.SetDestination(patrolDestination);
        }
        else
        {
            SetNewPatrolDestination();
        }
    }

    void ChasePlayer(float distanceToPlayer)
    {
        if (isAttacking)
        {
            agent.isStopped = true;
            SetAnimationState(0);
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.position);
        agent.speed = runSpeed;
        SetAnimationState(2); // Run animation

        float currentSpeed = agent.velocity.magnitude;
        animator.SetFloat(ANIM_SPEED, currentSpeed);

        // Face the direction of movement
        if (agent.velocity.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        }

        // Attack if in range
        if (CanAttack(distanceToPlayer))
        {
            StartCoroutine(PerformAttack());
        }
    }

    void SetAnimationState(int state)
    {
        // 0 = idle, 1 = walk, 2 = run
        if (animator != null)
        {
            animator.SetBool(ANIM_IS_WALKING, state == 1);
            animator.SetBool(ANIM_IS_RUNNING, state == 2);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        // Taking damage alerts enemy to player's presence
        hasDetectedPlayer = true;

        if (HP <= 0)
        {
            if (gamemanager.instance != null)
            {
                gamemanager.instance.updateGameGoal(-1);
            }
            Destroy(gameObject);
        }
    }

    bool CanAttack(float distanceToPlayer)
    {
        return !isAttacking && distanceToPlayer <= attackRange && Time.time >= nextAttackTime;
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        // Face player for attack
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        animator.SetTrigger(ANIM_ATTACK);

        // Wait for attack animation impact point
        yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

        // Deal damage if still in range
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            playerController playerHealth = player.GetComponent<playerController>();
            if (playerHealth != null)
            {
                playerHealth.takeDamage(meleeDamage);
            }
        }

        yield return new WaitForSeconds(attackAnimationDuration * 0.5f);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw FOV visualization
        Gizmos.color = Color.blue;
        Vector3 viewAngleA = DirectionFromAngle(transform.eulerAngles.y, -angleFOV / 2);
        Vector3 viewAngleB = DirectionFromAngle(transform.eulerAngles.y, angleFOV / 2);

        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + viewAngleA * detectionRange);
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + viewAngleB * detectionRange);
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}