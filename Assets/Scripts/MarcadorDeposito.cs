using UnityEngine;

// Ponlo en el objeto ZonaDeposito. 'visual' es el rayo o aro que el jugador debe ver.
public class MarcadorDeposito : MonoBehaviour
{
    [SerializeField] private Sistema sis;
    [SerializeField] private GameObject visual;

    [Header("Pulso")]
    [SerializeField] private float amplitud = 0.15f;
    [SerializeField] private float velocidadNormal = 3f;   // cuando lleva algo de carga
    [SerializeField] private float velocidadLleno = 7f;    // cuando la carga está llena

    private Vector3 escalaBase;

    private void Start()
    {
        if (visual != null) escalaBase = visual.transform.localScale;
    }

    private void Update()
    {
        if (visual == null) return;

        // Solo se ve cuando el dron lleva basura
        bool mostrar = sis.Carga > 0;
        if (visual.activeSelf != mostrar) visual.SetActive(mostrar);
        if (!mostrar) return;

        // Late más rápido cuando está lleno
        float velocidad = sis.EstaLleno ? velocidadLleno : velocidadNormal;
        float k = 1f + Mathf.Sin(Time.time * velocidad) * amplitud;
        visual.transform.localScale = new Vector3(escalaBase.x * k, escalaBase.y, escalaBase.z * k);
    }
}