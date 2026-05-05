using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeathManager : MonoBehaviour
{
    [Header("Fade")]
    public Image fadeImage;
    public float fadeSpeed = 2f;

    [Header("Respawn")]
    public Transform spawnPoint;
    public Transform xrOrigin;

    private bool isDead = false;

    void Update()
    {
        if (!isDead && GameManager.instance.currentLives <= 0)
        {
            StartCoroutine(FadeAndRespawn());
        }
    }

    IEnumerator FadeAndRespawn()
    {
        isDead = true;

        float alpha = 0;

        // Fade a negro
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // Resetear vidas
        GameManager.instance.currentLives = GameManager.instance.maxLives;

        // Obtener cámara
        Transform cameraTransform = xrOrigin.GetComponentInChildren<Camera>().transform;

        // Calcular offset
        Vector3 offset = cameraTransform.position - xrOrigin.position;

        // Reposicionar correctamente
        xrOrigin.position = spawnPoint.position - offset;
        xrOrigin.rotation = Quaternion.Euler(0, spawnPoint.rotation.eulerAngles.y, 0);

        // Fade de regreso
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        isDead = false;
    }
}