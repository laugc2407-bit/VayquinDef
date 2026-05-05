using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("Configuración")]
    public int dañoPorBala = 1;
    public float alcance = 30f;

    private Transform puntoDisparo;

    void Start()
    {
        if (Camera.main != null)
        {
            puntoDisparo = Camera.main.transform;
            Debug.Log("✅ Punto de disparo: " + puntoDisparo.name);
        }
        else
        {
            Debug.LogError("❌ No se encontró Camera.main");
        }
    }

    void Update()
    {
        // Presiona ESPACIO para disparar (funciona en el simulador)
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            Disparar();
    }

    public void Disparar()
    {
        if (puntoDisparo == null) return;

        Debug.Log("🔫 Disparo!");

        Ray rayo = new Ray(puntoDisparo.position, puntoDisparo.forward);
        Debug.DrawRay(rayo.origin, rayo.direction * alcance, Color.red, 1f);

        if (Physics.Raycast(rayo, out RaycastHit impacto, alcance))
        {
            Debug.Log("🎯 Golpeó: " + impacto.collider.name);
            EnemyHealth enemigo = impacto.collider.GetComponentInParent<EnemyHealth>();
            if (enemigo != null)
                enemigo.RecibirDaño(dañoPorBala);
        }
    }
}