using UnityEngine;

public class SpawnerHierba : MonoBehaviour
{
    [SerializeField] private Hierba hierbaBase;
    [SerializeField] private GameObject[] modelos;   // aquí arrastras tus modelos
    [SerializeField] private int cantidad = 100;
    [SerializeField] private Vector2 tamanoArea = new Vector2(100f, 100f);
    [SerializeField] private float altura = 0f;     
    [SerializeField] private float distanciaMinima = 5f;   // NUEVO: separación mínima entre piezas (0 = sin límite)

    private void Start()
    {
        for (int i = 0; i < cantidad; i++)
        {
            Hierba r = Instantiate(hierbaBase, PosicionAleatoria(), Quaternion.identity, transform);
            r.Inicializa(this);

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


        pos.y = aguaY;
        return pos;
    }

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
    }
}
