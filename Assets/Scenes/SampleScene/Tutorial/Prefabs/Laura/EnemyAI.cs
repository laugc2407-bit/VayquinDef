using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Rangos")]
    public float detectionRange = 20f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private EnemyHealth health;
    private Transform player;
    private float lastAttackTime;
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();

        // La Main Camera es lo que realmente se mueve con el jugador en VR
        if (Camera.main != null)
        {
            player = Camera.main.transform;
            Debug.Log("✅ Player = Main Camera en: " + player.position);
        }
        else
        {
            Debug.LogError("❌ No encontró Camera.main");
        }
    }

    void Update()
    {
        if (health != null && health.isDead) return;
        if (player == null) return;
        if (!agent.isOnNavMesh) return;

        float dist = Vector3.Distance(transform.position, player.position);
        Debug.Log($"Dist: {dist:F1} | Stopped: {agent.isStopped}");

        if (dist <= attackRange)
            Atacar();
        else if (dist <= detectionRange)
            Perseguir();
        else
            Idle();
    }

    void Perseguir()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position); // ya actualiza cada frame
        
        // Forzar que mire hacia el jugador mientras camina
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 5f
            );

        if (animator) animator.SetBool("isWalking", true);

        Debug.Log("🏃 Persiguiendo");
    }

    void Atacar()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        if (animator) animator.SetBool("isWalking", false);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (animator) animator.SetTrigger("Attack");
            GameManager.instance.TakeDamage(1);
            lastAttackTime = Time.time;
            Debug.Log("⚔️ Atacando");
        }
    }

    void Idle()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        if (animator) animator.SetBool("isWalking", false);
    }

    public void ResetEnemy()
    {
        // Resetear posición
        agent.Warp(startPosition); // Warp es la forma correcta de teletransportar un NavMeshAgent
        transform.rotation = startRotation;
        
        // Resetear estado
        agent.isStopped = false;
        if (animator) animator.SetBool("isWalking", false);
        
        // Resetear vida
        EnemyHealth health = GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.vidaActual = health.vidaMaxima;
            health.isDead = false;
        }
    }
}