using UnityEngine;

public class Recolectable : MonoBehaviour
{
    private SpawnerRecolectables spawner;
    [SerializeField] private Sistema sis;
    [SerializeField] private bool reaparecer = false;   // NUEVO: si es true, reaparece en otro punto al recogerla

    public void Inicializar(SpawnerRecolectables s)
    {
        spawner = s;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // NUEVO: si la carga está llena (o terminó la partida), no se recoge
        if (!sis.IntentarRecoger()) return;

        if (reaparecer)
        {
            spawner.Reubicar(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}