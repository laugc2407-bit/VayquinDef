using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Lives System")]
    public int maxLives = 3;
    public int currentLives;

    public int collectibles = 0;

    void Awake()
    {
        instance = this;

        // 🔥 CLAVE: iniciar con vidas completas
        currentLives = maxLives;

        Debug.Log("Vidas iniciales: " + currentLives);
    }

    public void TakeDamage(int damage)
    {
        currentLives -= 1; // 🔥 baja de a 1 vida

        currentLives = Mathf.Clamp(currentLives, 0, maxLives);

        Debug.Log("Vidas actuales: " + currentLives);

        DamageEffectManager.instance.ShowDamage();
    }

    public void Heal(int amount)
    {
        currentLives += amount;

        currentLives = Mathf.Clamp(currentLives, 0, maxLives);

        Debug.Log("Vida curada: " + currentLives);
    }

    public void AddCollectible(int amount)
    {
        collectibles += amount;
    }
}