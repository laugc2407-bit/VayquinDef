using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageEffectManager : MonoBehaviour
{
    public static DamageEffectManager instance;

    [Header("UI Panels")]
    public Image topPanel;
    public Image bottomPanel;

    [Header("Settings")]
    public float flashAlpha = 0.6f;
    public float lowHealthAlpha = 0.4f;
    public float fadeSpeed = 2f;

    private bool lowHealthActive = false;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        int lives = GameManager.instance.currentLives;

        // 🔴 Estado crítico: SOLO 1 vida
        if (lives == 1)
        {
            lowHealthActive = true;
            SetAlpha(lowHealthAlpha);
        }
        else
        {
            lowHealthActive = false;
        }
    }

    public void ShowDamage()
    {
        // Si ya está en estado crítico, no hacer flash
        if (lowHealthActive) return;

        StopAllCoroutines();
        StartCoroutine(DamageFlash());
    }

    IEnumerator DamageFlash()
    {
        // Aparece fuerte
        SetAlpha(flashAlpha);

        yield return new WaitForSeconds(0.2f);

        // Desaparece
        while (GetAlpha() > 0)
        {
            float newAlpha = GetAlpha() - Time.deltaTime * fadeSpeed;
            SetAlpha(newAlpha);
            yield return null;
        }

        SetAlpha(0);
    }

    // 🔧 Funciones auxiliares

    void SetAlpha(float alpha)
    {
        Color topColor = topPanel.color;
        Color bottomColor = bottomPanel.color;

        topColor.a = alpha;
        bottomColor.a = alpha;

        topPanel.color = topColor;
        bottomPanel.color = bottomColor;
    }

    float GetAlpha()
    {
        return topPanel.color.a;
    }
}