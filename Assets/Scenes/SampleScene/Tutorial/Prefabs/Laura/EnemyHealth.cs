using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configuración")]
    public int vidaMaxima = 3;
    public int vidaActual;
    public bool isDead = false;


    void Start()
    {
        vidaActual = vidaMaxima;
    }

    // Ambos métodos hacen lo mismo
    public void RecibirDaño(int daño) => TakeDamage(daño);

    public void TakeDamage(int daño)
    {
        if (isDead) return;
        vidaActual -= daño;
        Debug.Log($"💥 Enemigo recibió daño. Vida: {vidaActual}/{vidaMaxima}");

        if (vidaActual <= 0)
            Morir();
    }

    void Morir()
    {
        isDead = true;
        Debug.Log("💀 Enemigo eliminado");
        Destroy(gameObject);
    }
}