using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyController : MonoBehaviour, IDamage
{
    [Header("----- Enemy Settings -----")]
    [SerializeField] Renderer modelRenderer;
    [SerializeField] Material materialInstance;
    [SerializeField] Transform player;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] int HP;
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float detectionRange = 10f;
  
   

    [Header("----- Combat Settings -----")]
    [SerializeField] int meleeDamage = 20;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] float attackAnimationDuration = 0.5f;


    bool hasDetectedPlayer = false;
    float nextAttackTime;
    bool isAttacking;

    

    Vector3 startingPos;

    Color colorOrig;

    Coroutine co;
    

    // Animation parameter names
    private readonly string ANIM_IS_WALKING = "isWalking";
    private readonly string ANIM_IS_RUNNING = "isRunning";
    private readonly string ANIM_ATTACK = "Attack";
    private readonly string ANIM_SPEED = "Speed";

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        gameObject.tag = "Enemy";

        agent.stoppingDistance = attackRange;
        agent.speed = walkSpeed;

        // Initialize animation state
        SetAnimationState(1);

        materialInstance = new Material(modelRenderer.material);
        modelRenderer.material = materialInstance;
        colorOrig = materialInstance.color;

       if(gamemanager.instance != null)
       {
            gamemanager.instance.updateGameGoal(1);
       }

       startingPos = transform.position;

    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        UpdateMovementBehavior(distanceToPlayer);

        

        

        if (CanAttack(distanceToPlayer))
        {
            StartCoroutine(PerformAttack());
        }

        
    }

    

    void UpdateMovementBehavior(float distanceToPlayer)
    {
        if (isAttacking)
        {
            agent.isStopped = true;
            SetAnimationState(0); // Set to idle during attack
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.position);

        // Update movement animation based on agent's actual velocity
        float currentSpeed = agent.velocity.magnitude;
        animator.SetFloat(ANIM_SPEED, currentSpeed);

        // Check if player is within detection range
        if (distanceToPlayer <= detectionRange)
        {
            hasDetectedPlayer = true;
        }

        // Update movement state and speed
        if (hasDetectedPlayer)
        {
            agent.speed = runSpeed;
            SetAnimationState(2); // Run
        }
        else
        {
            agent.speed = walkSpeed;
            SetAnimationState(1); // Walk
        }

        // Ensure the enemy is facing the player
        if (agent.velocity.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        }
    }

    void SetAnimationState(int state)
    {
        // state: 0 = idle, 1 = walk, 2 = run
        if (animator != null)
        {
            animator.SetBool(ANIM_IS_WALKING, state == 1);
            animator.SetBool(ANIM_IS_RUNNING, state == 2);
        }
    }

    IEnumerator flashRed()
    {
        modelRenderer.sharedMaterial.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        modelRenderer.sharedMaterial.color = colorOrig;

    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        //if(player != null)
        //{
        //    agent.SetDestination(player.position);
        //}
        
        if(co != null)
        {
            StopCoroutine(co);
        }

        co = StartCoroutine(flashRed());

        if (HP <= 0)
        {
                gamemanager.instance.updateGameGoal(-1);
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

        // Ensure we're facing the player before attacking
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        // Trigger attack animation
        animator.SetTrigger(ANIM_ATTACK);

        // Wait for the animation to reach the impact point
        yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

        // Deal damage if player is still in range
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            playerController playerHealth = player.GetComponent<playerController>();
            if (playerHealth != null)
            {
                playerHealth.takeDamage(meleeDamage);
            }
        }

        // Wait for the rest of the animation
        yield return new WaitForSeconds(attackAnimationDuration * 0.5f);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

 
}