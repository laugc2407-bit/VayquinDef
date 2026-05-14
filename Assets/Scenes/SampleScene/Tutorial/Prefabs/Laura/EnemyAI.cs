using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;

    [Header("Rangos")]
    public float detectionRange = 10f;
    public float attackRange = 2f;

    [Header("Ataque")]
    public float attackCooldown = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private EnemyHealth health;
    private float lastAttackTime;
    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();

        if (player == null)
        {
            GameObject xrOrigin = GameObject.Find("XR Origin");

            if (xrOrigin != null)
            {
                player = xrOrigin.transform;
            }
            else
            {
                GameObject fallback = GameObject.FindGameObjectWithTag("Player");

                if (fallback != null)
                    player = fallback.transform;
                else
                    Debug.LogError("❌ No se encontró ni 'XR Origin' ni objeto con tag 'Player'");
            }
        }
    }

    void Update()
    {
        if (health != null && health.isDead) return;

        if (player == null)
        {
            Debug.LogWarning("⚠ EnemyAI: player sigue siendo NULL");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && !isAttacking)
        {
            AttackPlayer();
        }
        else if (distance <= detectionRange && !isAttacking)
        {
            ChasePlayer();
        }
        else if (!isAttacking)
        {
            Idle();
        }
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
        animator.SetBool("isWalking", true);
    }

    void AttackPlayer()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        animator.SetBool("isWalking", false);
        animator.SetTrigger("Attack");

        lastAttackTime = Time.time;
        isAttacking = true;

        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    void Idle()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        animator.SetBool("isWalking", false);
    }
}