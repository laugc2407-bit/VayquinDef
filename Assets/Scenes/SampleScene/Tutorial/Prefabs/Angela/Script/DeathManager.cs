using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeathManager : MonoBehaviour
{
    public static DeathManager instance;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeSpeed = 2f;

    [Header("Respawn")]
    public Transform[] spawnPoints;
    public Transform xrOrigin;

    private bool isDead = false;
    private int currentZoneIndex = 0;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!isDead && GameManager.instance.currentLives <= 0)
        {
            StartCoroutine(FadeAndRespawn());
        }
    }

    public void SetCurrentZone(int zoneIndex)
    {
        if (zoneIndex >= 0 && zoneIndex < spawnPoints.Length)
        {
            currentZoneIndex = zoneIndex;
        }
        else
        {
            Debug.LogWarning($"ZoneIndex {zoneIndex} fuera de rango. Revisa el array spawnPoints.");
        }
    }

    IEnumerator FadeAndRespawn()
    {
        isDead = true;

        float alpha = 0;

        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        GameManager.instance.currentLives = GameManager.instance.maxLives;

        Transform spawnPoint = spawnPoints[currentZoneIndex];
        Transform cameraTransform = xrOrigin.GetComponentInChildren<Camera>().transform;

        // 1. Rotar primero
        xrOrigin.rotation = Quaternion.Euler(0, spawnPoint.rotation.eulerAngles.y, 0);

        // 2. Mover al spawn point
        xrOrigin.position = spawnPoint.position;

        // 3. Corregir el offset que dejó la cámara en X y Z
        yield return null; // esperar un frame para que Unity actualice posiciones
        Vector3 diff = new Vector3(
            cameraTransform.position.x - spawnPoint.position.x,
            0,
            cameraTransform.position.z - spawnPoint.position.z
        );
        xrOrigin.position -= diff;

        Debug.Log($"[Respawn] Cámara DESPUÉS: {cameraTransform.position}");
        Debug.Log($"[Respawn] SpawnPoint era: {spawnPoint.position}");
        Debug.Log($"[Respawn] Diff corregido: {diff}");

        ResetZone(currentZoneIndex);

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        isDead = false;
    }

    void ResetZone(int zoneIndex)
    {
        Debug.Log($"Reiniciando zona {zoneIndex}");

        // Resetear todos los enemigos de la escena
        EnemyAI[] enemigos = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        foreach (EnemyAI enemigo in enemigos)
        {
            enemigo.gameObject.SetActive(true); // por si estaba muerto
            enemigo.ResetEnemy();
        }
    }
}