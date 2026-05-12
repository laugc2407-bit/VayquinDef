using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VRMenuSystem : MonoBehaviour
{
    private VRInputActions inputActions;

    [Header("Menu")]
    public GameObject menuCanvas;
    private bool isOpen = false;

    [Header("Health Images")]
    public Image[] vidas;

    [Header("UI Text")]
    public TextMeshProUGUI collectiblesText;

    [Header("Player")]
    public Transform playerCamera;



    void Awake()
    {
        inputActions = new VRInputActions();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.XRI.OpenMenu.performed += OnMenuPressed;
    }

    void OnDisable()
    {
        inputActions.XRI.OpenMenu.performed -= OnMenuPressed;
        inputActions.Disable();
    }

    void OnMenuPressed(InputAction.CallbackContext ctx)
    {
        Debug.Log("Botón presionado");
        ToggleMenu();
    }

    void ToggleMenu()
    {
        isOpen = !isOpen;
        menuCanvas.SetActive(isOpen);

        if (isOpen)
        {
            menuCanvas.transform.position = playerCamera.position + playerCamera.forward * 2.5f;
            menuCanvas.transform.LookAt(playerCamera);
            menuCanvas.transform.Rotate(0, 180f, 0);

            Time.timeScale = 0f;
            UpdateUI();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    void Update()
    {
        UpdateUI();


    }

    void UpdateUI()
    {
        UpdateHealth();
        collectiblesText.text = "=" + GameManager.instance.collectibles;
    }

    void UpdateHealth()
    {
        int vidasActuales = GameManager.instance.currentLives;

        for (int i = 0; i < vidas.Length; i++)
        {
            if (i < vidasActuales)
            {
                // Vida activa
                vidas[i].color = Color.white;
            }
            else
            {
                // Vida perdida (apagada)
                vidas[i].color = new Color(0, 0, 0, 0.6f);
            }
        }
    }   

}