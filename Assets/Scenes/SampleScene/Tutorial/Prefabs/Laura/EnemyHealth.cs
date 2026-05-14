using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Salud")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead = false;

    private Animator animator;
    private NavMeshAgent agent;
    private EnemyAI enemyAI;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyAI = GetComponent<EnemyAI>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"Enemigo recibió {amount} daño. Salud restante: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Activa tu Trigger Death → Standing ...Forward 01
        animator.SetBool("isWalking", false);
        animator.SetTrigger("Death");

        // Detiene todo movimiento
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.enabled = false;

        // Desactiva la IA
        enemyAI.enabled = false;

        // Desactiva el collider para que no siga siendo golpeado
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Destruye el GameObject después de que termine la animación
        Destroy(gameObject, 4f);
    }
}