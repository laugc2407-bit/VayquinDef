using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HealPickup : MonoBehaviour
{
    public int healAmount = 1;

    private void Awake()
    {
        // Asegúrate de que este objeto tenga un XRGrabInteractable
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        HealPlayer();
        DestroySelf();
    }

    public void HealPlayer()
    {
        GameManager.instance.Heal(healAmount);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}