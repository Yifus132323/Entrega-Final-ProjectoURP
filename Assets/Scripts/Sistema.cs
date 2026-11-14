using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Sistema : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI textoPuntuacion;                  // entregadas / meta (el contador de la tarjeta de misión)
    [SerializeField] private TextMeshProUGUI textoCarga;     // carga del dron, ej. "2/5"
    [SerializeField] private Slider barraBateria;            // batería del dron (tiempo)
    [SerializeField] private GameObject avisoLleno;          // NUEVO: aviso "dron lleno"
    [SerializeField] private float duracionAviso = 2f;       // NUEVO: segundos que se ve el aviso

    [Header("Reglas")]
    [SerializeField] private int capacidad = 5;              // basuras que el dron puede llevar a la vez
    [SerializeField] private int metaBasuras = 20;           // basuras estimadas para 3 estrellas
    [SerializeField] private float duracionBateria = 180f;   // segundos de batería
    [SerializeField, Range(0f, 1f)] private float porcentajeDosEstrellas = 0.6f;

    [Header("Fin de partida")]
    [SerializeField] private MovimientoDron dron;            // para apagarlo al terminar
    [SerializeField] private string escenaVictoria = "MCompletada";   // nombre exacto de tu escena
    [SerializeField] private float retardoFinal = 1.5f;      // segundos antes de cambiar de escena

    public int entregadas;       // basuras ya depositadas en el barco
    private int carga;           // basuras que lleva el dron ahora
    private float bateria;
    private float avisoHasta;    // NUEVO: momento hasta el que se muestra el aviso
    private bool terminado;

    // Lectura para el marcador y la flecha del barco
    public int Carga => carga;
    public bool EstaLleno => carga >= capacidad;
    public bool Terminado => terminado;   // NUEVO: lo lee la pausa

    private void Start()
    {
        bateria = duracionBateria;

        if (barraBateria != null)
        {
            barraBateria.minValue = 0f;
            barraBateria.maxValue = 1f;
        }

        if (avisoLleno != null) avisoLleno.SetActive(false);
        ActualizarUI();
    }

    private void Update()
    {
        if (terminado)
        {
            if (avisoLleno != null) avisoLleno.SetActive(false);
            return;
        }

        bateria -= Time.deltaTime;
        if (bateria <= 0f)
        {
            bateria = 0f;
            Terminar();
        }

        // NUEVO: el aviso se ve solo unos segundos después de intentar recoger con la carga llena
        if (avisoLleno != null) avisoLleno.SetActive(Time.time < avisoHasta);

        ActualizarUI();
    }

    // Lo llama Recolectable. Devuelve false si no puede recoger (carga llena o partida terminada).
    public bool IntentarRecoger()
    {
        if (terminado) return false;

        if (carga >= capacidad)
        {
            avisoHasta = Time.time + duracionAviso;   // NUEVO: dispara el aviso
            return false;
        }

        carga++;

        // NUEVO: avisa apenas se llena, sin esperar a que intente recoger otra
        if (carga >= capacidad) avisoHasta = Time.time + duracionAviso;

        return true;
    }

    // Lo llama ZonaDeposito cuando el dron llega al barco.
    public void Depositar()
    {
        if (terminado || carga == 0) return;

        entregadas += carga;
        carga = 0;

        if (entregadas >= metaBasuras)
        {
            Terminar();
        }
    }

    private void Terminar()
    {
        if (terminado) return;
        terminado = true;

        int numEstrellas;
        if (entregadas >= metaBasuras) numEstrellas = 3;
        else if (entregadas >= metaBasuras * porcentajeDosEstrellas) numEstrellas = 2;
        else numEstrellas = 1;

        // Guarda el resultado para que la escena de victoria lo lea
        ResultadoPartida.estrellas = numEstrellas;
        ResultadoPartida.entregadas = entregadas;
        ResultadoPartida.meta = metaBasuras;

        // Apaga el control del dron y lo frena
        if (dron != null)
        {
            dron.enabled = false;
            Rigidbody rb = dron.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;
        }

        ActualizarUI();
        Invoke(nameof(CargarEscenaFinal), retardoFinal);
    }

    private void CargarEscenaFinal()
    {
        SceneManager.LoadScene(escenaVictoria);
    }

    private void ActualizarUI()
    {
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text = entregadas + "/" + metaBasuras;
        }

        if (textoCarga != null)
        {
            textoCarga.text = carga + "/" + capacidad;
            // Rojo cuando está lleno, blanco en los demás casos
            textoCarga.color = EstaLleno ? new Color(1f, 0.35f, 0.35f) : Color.white;
        }

        if (barraBateria != null)
        {
            barraBateria.value = bateria / duracionBateria;
        }
    }
}