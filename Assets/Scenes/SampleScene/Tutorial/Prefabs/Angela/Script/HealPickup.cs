using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HealPickup : MonoBehaviour
{
    public int healAmount = 1;

    [Header("Sonido")]
    public AudioClip pickupSound;
    public float volume = 1f;

    private void Awake()
    {
        // Obtener el XRGrabInteractable
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable =
            GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        HealPlayer();

        // Reproducir sonido
        PlayPickupSound();

        DestroySelf();
    }

    public void HealPlayer()
    {
        GameManager.instance.Heal(healAmount);
    }

    private void PlayPickupSound()
    {
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}