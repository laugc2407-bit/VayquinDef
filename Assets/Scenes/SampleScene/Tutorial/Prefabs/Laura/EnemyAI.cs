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

    private NavMeshAgent agente;
    private float timerAtaque = 0f;

    private enum Estado { Idle, Persiguiendo, Atacando }
    private Estado estadoActual = Estado.Idle;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();

        // Busca la CÁMARA del XR Origin (posición real de la cabeza)
        if (jugador == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                jugador = cam.transform;
                Debug.Log("✅ Jugador asignado a: " + jugador.name);
            }
            else
            {
                Debug.LogError("❌ No se encontró Camera.main");
            }
        }

        if (agente != null)
            Debug.Log("✅ NavMeshAgent encontrado");
    }

    void Update()
    {
        if (jugador == null || agente == null) return;

        // Distancia solo en el plano horizontal (ignora altura)
        Vector3 posEnemigo = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 posJugador = new Vector3(jugador.position.x, 0, jugador.position.z);
        float distancia = Vector3.Distance(posEnemigo, posJugador);

        timerAtaque += Time.deltaTime;

        Debug.Log($"📍 Distancia: {distancia:F1} | Estado: {estadoActual}");

        switch (estadoActual)
        {
            case Estado.Idle:
                agente.ResetPath();
                if (distancia <= distanciaDeteccion)
                {
                    estadoActual = Estado.Persiguiendo;
                    Debug.Log("👁 Enemigo detectó al jugador");
                }
                break;

            case Estado.Persiguiendo:
                // Destino en el suelo, no en la cabeza VR
                Vector3 destinoEnSuelo = new Vector3(jugador.position.x, transform.position.y, jugador.position.z);
                agente.SetDestination(destinoEnSuelo);

                if (distancia <= distanciaAtaque)
                {
                    agente.ResetPath();
                    estadoActual = Estado.Atacando;
                }
                else if (distancia > distanciaDeteccion * 1.5f)
                {
                    estadoActual = Estado.Idle;
                }
                break;

            case Estado.Atacando:
                agente.ResetPath();

                // Mira al jugador
                Vector3 dir = (jugador.position - transform.position).normalized;
                dir.y = 0;
                if (dir != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);

                if (timerAtaque >= tiempoEntreAtaques)
                {
                    Atacar();
                    timerAtaque = 0f;
                }

                if (distancia > distanciaAtaque)
                    estadoActual = Estado.Persiguiendo;
                break;
        }
    }

    void Atacar()
    {
        Debug.Log("⚔️ Enemigo ataca");
        if (GameManager.instance != null)
            GameManager.instance.TakeDamage(dañoPorAtaque);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}