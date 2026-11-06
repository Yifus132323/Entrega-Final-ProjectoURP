using UnityEngine;

public class SpawnerRecolectables : MonoBehaviour
{
    [SerializeField] private Recolectable prefabBase;
    [SerializeField] private GameObject[] modelos;   // aquí arrastras tus modelos
    [SerializeField] private int cantidad = 100;
    [SerializeField] private Vector2 tamanoArea = new Vector2(100f, 100f);
    [SerializeField] private float altura = 0f;

    private void Start()
    {
        for (int i = 0; i < cantidad; i++)
        {
            Recolectable r = Instantiate(prefabBase, PosicionAleatoria(), Quaternion.identity, transform);
            r.Inicializar(this);

            GameObject modelo = Instantiate(modelos[Random.Range(0, modelos.Length)], r.transform);
            modelo.transform.localPosition = Vector3.zero;
        }
    }

    public Vector3 PosicionAleatoria()
    {
        float x = Random.Range(-tamanoArea.x / 2f, tamanoArea.x / 2f);
        float z = Random.Range(-tamanoArea.y / 2f, tamanoArea.y / 2f);
        return transform.position + new Vector3(x, altura, z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + Vector3.up * altura,
                            new Vector3(tamanoArea.x, 0.1f, tamanoArea.y));
    }
}