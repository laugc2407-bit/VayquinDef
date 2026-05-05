using UnityEngine;

public class Inicio : MonoBehaviour
{
    [Header("Asignar en el Inspector")]
    public GameObject canvasInstrucciones; // El Canvas con el panel e instrucciones
    public AudioSource sonido;             // El sonido que quieres reproducir

    private bool yaMostrado = false;

    private void OnTriggerEnter(Collider other)
    {
        // Si ya se mostró, no volver a mostrar
        if (!yaMostrado)
        {
            canvasInstrucciones.SetActive(true);
            if (sonido != null)
            {
                sonido.Play();
            }
            yaMostrado = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Al salir de la zona, ocultar el panel y detener el sonido
        if (canvasInstrucciones.activeSelf)
        {
            canvasInstrucciones.SetActive(false);
        }
        if (sonido != null && sonido.isPlaying)
        {
            sonido.Stop();
        }
    }
}