using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Arma de Raycast para VR — presiona F para disparar.
/// 
/// Setup en Inspector:
///   - Arrastra la cámara principal (o el arma) en "origenDisparo"
///   - El enemigo DEBE tener un Collider (no trigger) y el script EnemyAI
///   - Asegúrate de que el Collider esté en el GameObject raíz del enemigo
///     o en un hijo con el script EnemyAI en el padre (usamos GetComponentInParent)
/// </summary>
public class RaycastWeapon : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Desde dónde sale el rayo. Arrastra aquí la cámara VR o el cañón del arma.")]
    public Transform origenDisparo;

    [Tooltip("Daño por disparo.")]
    public int dañoPorDisparo = 25;

    [Tooltip("Alcance máximo del disparo en metros.")]
    public float alcanceMaximo = 50f;

    [Tooltip("Capas que el rayo puede golpear. Incluye la capa del enemigo.")]
    public LayerMask capasObjetivo = Physics.DefaultRaycastLayers;

    [Header("Feedback Visual (opcional)")]
    [Tooltip("Partícula o efecto de impacto. Puede quedar vacío.")]
    public GameObject efectoImpacto;

    [Header("Debug")]
    public bool mostrarRayoEnEditor = true;

    void Start()
    {
        // Si no se asignó origen, usar la cámara principal
        if (origenDisparo == null)
        {
            if (Camera.main != null)
            {
                origenDisparo = Camera.main.transform;
                Debug.Log("✅ origenDisparo asignado a Camera.main");
            }
            else
            {
                Debug.LogError("❌ No hay Camera.main. Asigna origenDisparo manualmente en el Inspector.");
            }
        }
    }

    void Update()
    {
        // Nuevo Input System: Keyboard.current en lugar de Input.GetKeyDown
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            Disparar();
    }

    void Disparar()
    {
        if (origenDisparo == null) return;

        Ray rayo = new Ray(origenDisparo.position, origenDisparo.forward);
        RaycastHit impacto;

        Debug.DrawRay(rayo.origin, rayo.direction * alcanceMaximo, Color.red, 1f);

        if (Physics.Raycast(rayo, out impacto, alcanceMaximo, capasObjetivo))
        {
            Debug.Log($"🎯 Rayo golpeó: {impacto.collider.gameObject.name} " +
                      $"(layer: {LayerMask.LayerToName(impacto.collider.gameObject.layer)})");

            // Buscar EnemyAI en el objeto golpeado o en su padre
            // (cubre el caso en que el Collider está en un mesh hijo)
            EnemyAI enemigo = impacto.collider.GetComponent<EnemyAI>()
                           ?? impacto.collider.GetComponentInParent<EnemyAI>();

            if (enemigo != null)
            {
                enemigo.RecibirDaño(dañoPorDisparo);
                Debug.Log($"💥 Golpeó al enemigo: {enemigo.name}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Se golpeó '{impacto.collider.gameObject.name}' " +
                                 "pero no tiene EnemyAI. ¿El Collider está en el objeto correcto?");
            }

            // Efecto de impacto visual
            if (efectoImpacto != null)
                Instantiate(efectoImpacto, impacto.point, Quaternion.LookRotation(impacto.normal));
        }
        else
        {
            Debug.Log("💨 El rayo no golpeó nada. Revisa que el enemigo tenga Collider " +
                      "y que su layer esté incluido en 'Capas Objetivo'.");
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!mostrarRayoEnEditor || origenDisparo == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(origenDisparo.position, origenDisparo.forward * alcanceMaximo);
    }
#endif
}