using UnityEngine;

// Ponlo en un objeto vacío dentro de la escena MCompletada.
public class PantallaVictoria : MonoBehaviour
{
    [SerializeField] private GameObject[] estrellas;   // las 3 estrellas de tu pantalla, de izquierda a derecha

    private void Start()
    {
        for (int i = 0; i < estrellas.Length; i++)
        {
            if (estrellas[i] != null)
            {
                estrellas[i].SetActive(i < ResultadoPartida.estrellas);
            }
        }
    }
}