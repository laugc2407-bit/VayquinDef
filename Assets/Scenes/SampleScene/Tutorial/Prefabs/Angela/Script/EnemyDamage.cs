using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entró: " + other.name);

        if (other.transform.root.name.Contains("XR Origin"))
        {
            // ✅ No daña si el enemigo ya está muerto
            EnemyHealth health = GetComponentInParent<EnemyHealth>();
            if (health != null && health.isDead) return;

            Debug.Log("Golpe al jugador");
            GameManager.instance.TakeDamage(damage);
        }
    }
}