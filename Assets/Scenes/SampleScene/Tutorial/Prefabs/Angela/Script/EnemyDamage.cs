using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entró: " + other.name);

        // 🔥 Detecta si pertenece al XR Origin
        if (other.transform.root.name.Contains("XR Origin"))
        {
            Debug.Log("Golpe al jugador");

            GameManager.instance.TakeDamage(damage);
        }
    }

    
}