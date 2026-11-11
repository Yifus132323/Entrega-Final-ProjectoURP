using UnityEngine;

public class SpawnerRecolectables : MonoBehaviour
{
    [SerializeField] private Recolectable prefabBase;
    [SerializeField] private GameObject[] modelos;   // aquí arrastras tus modelos
    [SerializeField] private int cantidad = 100;
    [SerializeField] private Vector2 tamanoArea = new Vector2(100f, 100f);
    [SerializeField] private float altura = 0f;      // altura de la superficie del agua
    [SerializeField] private float distanciaMinima = 5f;   // NUEVO: separación mínima entre piezas (0 = sin límite)

    [Header("Profundidad")]
    [SerializeField, Range(0f, 1f)] private float porcentajeEnSuperficie = 0.3f; // NUEVO: cuánta basura queda arriba
    [SerializeField] private float profundidadMin = 1f;    // NUEVO
    [SerializeField] private float profundidadMax = 15f;   // NUEVO: igual a Profundidad Max del dron
    [SerializeField] private float margenSuelo = 0.5f;     // NUEVO: separación mínima del fondo

    private void Start()
    {
        for (int i = 0; i < cantidad; i++)
        {
            Recolectable r = Instantiate(prefabBase, PosicionAleatoria(), Quaternion.identity, transform);
            r.Inicializar(this);

            // NUEVO: si quedó bajo el agua, apagamos el flotador para que no la suba a la superficie
            if (r.transform.position.y < altura - 0.5f)
            {
                Behaviour flotador = r.GetComponent("WateverVolumeFloater1") as Behaviour;
                if (flotador != null) flotador.enabled = false;
            }

            GameObject modelo = Instantiate(modelos[Random.Range(0, modelos.Length)], r.transform);
            modelo.transform.localPosition = Vector3.zero;
        }
    }

    public Vector3 PosicionAleatoria()
    {
        // NUEVO: reintenta hasta encontrar un punto separado de las demás piezas
        Vector3 pos = transform.position;
        for (int intento = 0; intento < 30; intento++)
        {
            float x = Random.Range(-tamanoArea.x / 2f, tamanoArea.x / 2f);
            float z = Random.Range(-tamanoArea.y / 2f, tamanoArea.y / 2f);
            pos = transform.position + new Vector3(x, 0f, z);
            if (EsLejosDeOtros(pos)) break;
        }
        float aguaY = transform.position.y + altura;

        // NUEVO: una parte queda en superficie y el resto a profundidad aleatoria
        float profundidad = 0f;
        if (Random.value > porcentajeEnSuperficie)
        {
            float maxDisponible = profundidadMax;

            // NUEVO: no enterrar la basura dentro del terreno
            Terrain t = Terrain.activeTerrain;
            if (t != null)
            {
                float suelo = t.SampleHeight(pos) + t.transform.position.y;
                maxDisponible = Mathf.Min(profundidadMax, aguaY - suelo - margenSuelo);
            }

            if (maxDisponible > profundidadMin)
            {
                profundidad = Random.Range(profundidadMin, maxDisponible);
            }
        }

        pos.y = aguaY - profundidad;
        return pos;
    }

    // NUEVO: compara en el plano horizontal con las piezas que ya existen
    private bool EsLejosDeOtros(Vector3 pos)
    {
        if (distanciaMinima <= 0f) return true;

        float minSqr = distanciaMinima * distanciaMinima;
        foreach (Transform hijo in transform)
        {
            Vector3 d = hijo.position - pos;
            d.y = 0f;
            if (d.sqrMagnitude < minSqr) return false;
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + Vector3.up * altura,
                            new Vector3(tamanoArea.x, 0.1f, tamanoArea.y));

        // NUEVO: volumen donde puede aparecer basura bajo el agua
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + Vector3.up * (altura - profundidadMax / 2f),
                            new Vector3(tamanoArea.x, profundidadMax, tamanoArea.y));
    }
}