using UnityEngine;

public class ShurikenProjectile : MonoBehaviour
{
    public int daño = 1;
    public float velocidadRotacion = 720f;
    public float tiempoAutoDestruir = 5f;

    void Start()
    {
        Destroy(gameObject, tiempoAutoDestruir);

        TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 0.3f;
        trail.startWidth = 0.05f;
        trail.endWidth = 0f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update()
    {
        transform.Rotate(Vector3.forward, velocidadRotacion * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemigo"))
        {
            EnemyHealth enemigo = other.GetComponentInParent<EnemyHealth>();

            if (enemigo != null)
            {
                enemigo.TakeDamage(daño);
            }

            Destroy(gameObject);
            return;
        }

        if (other.isTrigger) return;
        if (other.CompareTag("Player")) return;

        Destroy(gameObject);
    }
}