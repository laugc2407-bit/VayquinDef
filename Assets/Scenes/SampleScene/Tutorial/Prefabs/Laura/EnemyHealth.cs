using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configuración")]
    public int vidaMaxima = 3;
    private int vidaActual;

    void Start()
    {
        vidaActual = vidaMaxima;
    }

    public void RecibirDaño(int daño)
    {
        vidaActual -= daño;
        Debug.Log($"💥 Enemigo recibió daño. Vida: {vidaActual}/{vidaMaxima}");

        if (vidaActual <= 0)
            Morir();
    }

    void Morir()
    {
        Debug.Log("💀 Enemigo eliminado");
        Destroy(gameObject);
    }
}