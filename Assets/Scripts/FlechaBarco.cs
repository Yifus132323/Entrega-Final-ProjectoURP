using TMPro;
using UnityEngine;

// Ponlo en la imagen de la flecha (UI > Image) dentro del Canvas.
[RequireComponent(typeof(CanvasGroup))]
public class FlechaBarco : MonoBehaviour
{
    [SerializeField] private Sistema sis;
    [SerializeField] private Transform dron;
    [SerializeField] private Transform destino;      // el objeto ZonaDeposito (o el Barco)
    [SerializeField] private Transform camara;       // la Main Camera
    [SerializeField] private TextMeshProUGUI textoDistancia;   // opcional: muestra "42 m"

    [Header("Cuándo se ve")]
    [SerializeField] private bool soloCuandoLleno = true;      // false = se ve con cualquier carga
    [SerializeField] private float distanciaOcultar = 8f;      // se oculta al estar cerca del barco

    private CanvasGroup grupo;

    private void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        Vector3 haciaDestino = destino.position - dron.position;
        haciaDestino.y = 0f;
        float distancia = haciaDestino.magnitude;

        bool mostrar = soloCuandoLleno ? sis.EstaLleno : sis.Carga > 0;
        if (distancia < distanciaOcultar) mostrar = false;

        grupo.alpha = mostrar ? 1f : 0f;

        if (textoDistancia != null)
        {
            textoDistancia.gameObject.SetActive(mostrar);
            if (mostrar) textoDistancia.text = Mathf.RoundToInt(distancia) + " m";
        }

        if (!mostrar) return;

        // Ángulo entre hacia dónde mira la cámara y la dirección al barco
        Vector3 frente = camara.forward;
        frente.y = 0f;
        float angulo = Vector3.SignedAngle(frente, haciaDestino, Vector3.up);

        // Una flecha de UI que apunta hacia arriba gira al revés que el ángulo del mundo
        transform.localRotation = Quaternion.Euler(0f, 0f, -angulo);
    }
}