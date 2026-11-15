using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Ponlo en un objeto que SIEMPRE esté activo (por ejemplo un objeto vacío "PausaManager"),
// nunca en el panel de pausa, porque un objeto apagado no ejecuta su Update.
public class PausaJuego : MonoBehaviour
{
    [SerializeField] private GameObject panelPausa;
    [SerializeField] private GameObject botonInicial;   // botón que queda seleccionado para el mando (Reanudar)
    [SerializeField] private Sistema sis;               // para no pausar cuando la partida ya terminó
    [SerializeField] private GameObject[] ocultarAlPausar;   // NUEVO: elementos del HUD que se esconden mientras está la pausa

    private bool pausado;

    private void Start()
    {
        Reanudar();   // empieza siempre sin pausa
    }

    private void Update()
    {
        // Botón Start (PS4/PS5: Options) del mando o tecla Esc
        bool pulsado =
            (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame) ||
            (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame);

        if (pulsado) Alternar();
    }

    // También se llama desde el botón de pausa de la pantalla (OnClick)
    public void Alternar()
    {
        if (pausado) Reanudar();
        else Pausar();
    }

    public void Pausar()
    {
        if (sis != null && sis.Terminado) return;

        pausado = true;
        Time.timeScale = 0f;
        if (panelPausa != null) panelPausa.SetActive(true);
        MostrarHUD(false);   // NUEVO: esconde los contenedores

        // Deja un botón seleccionado para poder navegar con el mando
        if (botonInicial != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(botonInicial);
        }
    }

    public void Reanudar()
    {
        pausado = false;
        Time.timeScale = 1f;
        if (panelPausa != null) panelPausa.SetActive(false);
        MostrarHUD(true);    // NUEVO: vuelve a mostrar los contenedores
    }

    // NUEVO: activa o desactiva los elementos del HUD que arrastraste a la lista
    private void MostrarHUD(bool mostrar)
    {
        if (ocultarAlPausar == null) return;

        foreach (GameObject elemento in ocultarAlPausar)
        {
            if (elemento != null) elemento.SetActive(mostrar);
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;   // seguridad al cambiar de escena
    }
}