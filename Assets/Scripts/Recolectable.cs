using UnityEngine;

public class Recolectable : MonoBehaviour
{
    private SpawnerRecolectables spawner;
    [SerializeField] private Sistema sis;

    public void Inicializar(SpawnerRecolectables s)
    {
        spawner = s;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sis.puntos++;
            transform.position = spawner.PosicionAleatoria();
        }



        
    }
}