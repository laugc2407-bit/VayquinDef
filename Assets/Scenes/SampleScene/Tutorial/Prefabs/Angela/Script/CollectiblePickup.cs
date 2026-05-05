using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CollectiblePickup : MonoBehaviour
{
    public int collectibleValue = 1;

    private void Awake()
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        AddCollectible();
        DestroySelf();
    }

    public void AddCollectible()
    {
        GameManager.instance.AddCollectible(collectibleValue);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
