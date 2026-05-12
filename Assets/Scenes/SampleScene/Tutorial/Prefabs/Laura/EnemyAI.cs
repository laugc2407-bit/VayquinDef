
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;

    [Header("Configuración")]
    public float distanciaDeteccion = 8f;
    public float distanciaAtaque = 1.8f;
    public float tiempoEntreAtaques = 1.5f;
    public int dañoPorAtaque = 10;
    public int vidaMaxima = 100;

    private NavMeshAgent agente;
    private Animator animator;
    private Rigidbody rb;
    private float timerAtaque = 0f;
    private int vidaActual;
    private bool estaMuerto = false;

    private enum Estado { Idle, Persiguiendo, Atacando, Muerto }
    private Estado estadoActual = Estado.Idle;

    private const string PARAM_IS_WALKING = "isWalking";
    private const string TRIGGER_ATTACK = "Attack";
    private const string TRIGGER_DEATH = "Death";

    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        vidaActual = vidaMaxima;

        // FIX A: Rigidbody + NavMeshAgent → dejar que el agente controle la física
        if (rb != null)
        {
            rb.isKinematic = true;          // El NavMeshAgent mueve el transform; el Rigidbody solo colisiona
            rb.useGravity = false;         // El agente ya maneja la gravedad sobre el NavMesh
            rb.constraints = RigidbodyConstraints.FreezeAll; // Evita que físicas externas lo desplacen
            Debug.Log("✅ Rigidbody configurado como Kinematic para coexistir con NavMeshAgent");
        }

        // Validar NavMeshAgent
        if (agente == null) { Debug.LogError("❌ Falta NavMeshAgent en " + name); enabled = false; return; }

        if (!agente.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                Debug.Log("✅ Reposicionado sobre NavMesh: " + hit.position);
            }
            else
            {
                Debug.LogError("❌ No hay NavMesh cerca del enemigo. Hornea el NavMesh.");
                enabled = false; return;
            }
        }

        // Validar Animator
        if (animator == null || animator.runtimeAnimatorController == null)
            Debug.LogWarning("⚠️ Animator no encontrado o sin Controller en " + name);

        // Buscar jugador
        if (jugador == null) BuscarJugador();

        // FIX B: Registrar este script en todos los colliders hijos
        // para que el Raycast que golpee cualquier parte del cuerpo funcione
        RegistrarColisionesHijas();
    }

    // FIX B: Añade EnemyHitProxy en cada Collider hijo que no tenga este script
    void RegistrarColisionesHijas()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(includeInactive: true);
        foreach (Collider col in colliders)
        {
            if (col.gameObject == gameObject) continue; // el raíz ya tiene este script
            if (col.gameObject.GetComponent<EnemyHitProxy>() == null)
            {
                EnemyHitProxy proxy = col.gameObject.AddComponent<EnemyHitProxy>();
                proxy.enemyAI = this;
            }
        }
        Debug.Log($"✅ {colliders.Length} collider(s) enlazados al EnemyAI");
    }

    // ─────────────────────────────────────────────────────────────
    void BuscarJugador()
    {
        string[] tags = { "Player", "XRRig", "XR Rig" };
        foreach (string tag in tags)
        {
            try
            {
                GameObject obj = GameObject.FindGameObjectWithTag(tag);
                if (obj != null) { jugador = obj.transform; Debug.Log($"✅ Jugador: {obj.name}"); return; }
            }
            catch { }
        }
        if (Camera.main != null)
        {
            jugador = Camera.main.transform;
            Debug.LogWarning("⚠️ Usando Camera.main como jugador. Asigna XR Origin si usas XR Toolkit.");
        }
        else Debug.LogError("❌ No se encontró jugador. Asígnalo en el Inspector.");
    }

    // ─────────────────────────────────────────────────────────────
    void Update()
    {
        if (estaMuerto || jugador == null || agente == null || !agente.isOnNavMesh) return;

        float distancia = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(jugador.position.x, 0, jugador.position.z));

        timerAtaque += Time.deltaTime;

        switch (estadoActual)
        {
            case Estado.Idle:
                agente.ResetPath();
                SetBool(PARAM_IS_WALKING, false);
                if (distancia <= distanciaDeteccion)
                {
                    estadoActual = Estado.Persiguiendo;
                    Debug.Log("👁 Enemigo detectó al jugador");
                }
                break;

            case Estado.Persiguiendo:
                agente.SetDestination(jugador.position);
                SetBool(PARAM_IS_WALKING, agente.velocity.magnitude > 0.1f);

                if (distancia <= distanciaAtaque) { agente.ResetPath(); estadoActual = Estado.Atacando; }
                else if (distancia > distanciaDeteccion * 1.5f) estadoActual = Estado.Idle;
                break;

            case Estado.Atacando:
                agente.ResetPath();
                SetBool(PARAM_IS_WALKING, false);

                Vector3 dir = jugador.position - transform.position; dir.y = 0;
                if (dir.sqrMagnitude > 0.01f)
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                        Quaternion.LookRotation(dir.normalized), Time.deltaTime * 5f);

                if (timerAtaque >= tiempoEntreAtaques) { Atacar(); timerAtaque = 0f; }
                if (distancia > distanciaAtaque) estadoActual = Estado.Persiguiendo;
                break;
        }
    }

    // ─────────────────────────────────────────────────────────────
    void Atacar()
    {
        Debug.Log("⚔️ Ataque");
        SetTrigger(TRIGGER_ATTACK);
        if (GameManager.instance != null)
            GameManager.instance.TakeDamage(dañoPorAtaque);
    }

    // Llámado desde EnemyHitProxy o directamente desde el Raycast
    public void RecibirDaño(int daño)
    {
        if (estaMuerto) return;
        vidaActual -= daño;
        Debug.Log($"💢 Daño: {daño} | Vida restante: {vidaActual}/{vidaMaxima}");
        if (vidaActual <= 0) Morir();
    }

    void Morir()
    {
        estaMuerto = true; estadoActual = Estado.Muerto;
        Debug.Log("💀 Muerto");
        SetTrigger(TRIGGER_DEATH);
        if (agente != null) { agente.ResetPath(); agente.enabled = false; }
        if (rb != null) rb.isKinematic = false; // Opcional: ragdoll al morir
        Destroy(gameObject, 3f);
    }

    void SetBool(string p, bool v) { if (animator && animator.runtimeAnimatorController) animator.SetBool(p, v); }
    void SetTrigger(string p) { if (animator && animator.runtimeAnimatorController) animator.SetTrigger(p); }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}


// ============================================================
//  EnemyHitProxy.cs  —  Se añade automáticamente a cada
//  Collider hijo para propagar el daño del Raycast al EnemyAI
// ============================================================
public class EnemyHitProxy : MonoBehaviour
{
    [HideInInspector] public EnemyAI enemyAI;

    public void RecibirDaño(int daño)
    {
        if (enemyAI != null)
            enemyAI.RecibirDaño(daño);
    }
}