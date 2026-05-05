using UnityEngine;

public class VRZoneSwitcher : MonoBehaviour
{
    [Header("Zonas")]
    public GameObject zonaActual;
    public GameObject zonaSiguiente;

    [Header("Jugador VR")]
    public Transform xrRig;

    [Header("Spawn en nueva zona")]
    public Transform puntoDestino;

    [Header("Opciones")]
    public bool desactivarZonaActual = true;
    public bool activarZonaSiguiente = true;

    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activado) return;

        if (other.CompareTag("Player"))
        {
            activado = true;
            CambiarZona();
        }
    }

    void CambiarZona()
    {
        // 1. Desactivar zona actual
        if (desactivarZonaActual && zonaActual != null)
        {
            zonaActual.SetActive(false);
        }

        // 2. Activar zona siguiente
        if (activarZonaSiguiente && zonaSiguiente != null)
        {
            zonaSiguiente.SetActive(true);
        }

        // 3. Teleport seguro VR (SIN offset)
        if (xrRig != null && puntoDestino != null)
        {
            xrRig.position = puntoDestino.position;
        }
    }
}