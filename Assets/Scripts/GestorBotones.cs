using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Un solo script para todos los botones del juego.
// - En los menús (Start, Niveles, MCompletada): úsalo solo con CargarEscena / ReiniciarEscenaActual / SalirDelJuego
//   y deja vacíos los campos de pausa.
// - En Scene1: asigna también los campos de pausa. Ponlo en un objeto que SIEMPRE esté activo (ej. "PausaManager").
public class GestorBotones : MonoBehaviour
{
    [Header("Pausa (solo en la escena del juego)")]
    [SerializeField] private GameObject panelPausa;
    [SerializeField] private GameObject botonInicial;          // botón que queda seleccionado para el mando (Jugar)
    [SerializeField] private Sistema sis;                      // para no pausar si la partida ya terminó
    [SerializeField] private GameObject[] ocultarAlPausar;     // contenedores del HUD que se esconden al pausar

    [Header("Mando en menús (Start, Niveles, MCompletada)")]
    [SerializeField] private GameObject primerBoton;           // botón que queda seleccionado al abrir la escena; NO lo uses en Scene1
    [SerializeField] private string escenaAlCancelar;          // si lo llenas (ej. Start), Círculo / B / Esc vuelven a esa escena; déjalo vacío en Scene1

    private bool pausado;

    private void Start()
    {
        Time.timeScale = 1f;
        if (panelPausa != null) Reanudar();   // empieza siempre sin pausa

        // En los menús, deja un botón seleccionado para poder usar el mando desde el inicio
        if (primerBoton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(primerBoton);
        }
    }

    private void Update()
    {
        RecuperarSeleccion();

        // Botón "atrás" de los menús: Círculo (PS4/PS5), B (Xbox) o Esc
        if (!string.IsNullOrEmpty(escenaAlCancelar))
        {
            bool atras =
                (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame);

            if (atras)
            {
                CargarEscena(escenaAlCancelar);
                return;
            }
        }

        // Si no hay panel de pausa asignado (menús), no se escucha el botón de pausa
        if (panelPausa == null) return;

        // Botón Start (PS4/PS5: Options) del mando o tecla Esc
        bool pulsado =
            (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame) ||
            (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame);

        if (pulsado) Alternar();
    }

    // Si hiciste clic en un espacio vacío con el mouse, se pierde la selección y el mando deja de navegar.
    // Esto la recupera en cuanto mueves el stick o la cruceta (solo en menús o con la pausa abierta).
    private void RecuperarSeleccion()
    {
        if (EventSystem.current == null) return;
        if (EventSystem.current.currentSelectedGameObject != null) return;

        GameObject objetivo = pausado ? botonInicial : primerBoton;
        if (objetivo == null || !objetivo.activeInHierarchy) return;

        Gamepad pad = Gamepad.current;
        if (pad == null) return;

        bool mueve = pad.leftStick.ReadValue().sqrMagnitude > 0.25f
                  || pad.dpad.ReadValue().sqrMagnitude > 0.1f;

        if (mueve) EventSystem.current.SetSelectedGameObject(objetivo);
    }

    // ---------- CAMBIO DE ESCENA ----------

    // OnClick (Static Parameters) y escribes el nombre exacto de la escena, por ejemplo: Niveles
    public void CargarEscena(string nombreEscena)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }

    // Vuelve a cargar la escena actual (reinicia la misión)
    public void ReiniciarEscenaActual()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Cierra el juego (en el editor solo detiene el Play)
    public void SalirDelJuego()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ---------- PAUSA ----------

    public void Alternar()
    {
        if (pausado) Reanudar();
        else Pausar();
    }

    public void Pausar()
    {
        if (panelPausa == null) return;
        if (sis != null && sis.Terminado) return;

        pausado = true;
        Time.timeScale = 0f;
        panelPausa.SetActive(true);
        MostrarHUD(false);

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
        MostrarHUD(true);
    }

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