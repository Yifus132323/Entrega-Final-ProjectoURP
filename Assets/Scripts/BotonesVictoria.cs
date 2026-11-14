using UnityEngine;
using UnityEngine.SceneManagement;

// Ponlo en el objeto PantallaVictoria (o en un objeto vacío) de la escena MCompletada.
public class BotonesVictoria : MonoBehaviour
{
    [SerializeField] private string escenaNiveles = "Niveles";
    [SerializeField] private string escenaJuego = "Scene1";     // la escena del nivel que se reinicia
    [SerializeField] private string escenaInicio = "Start";

    public void Jugar()
    {
        Cargar(escenaNiveles);
    }

    public void ReiniciarMision()
    {
        Cargar(escenaJuego);
    }

    public void Salir()
    {
        Cargar(escenaInicio);
    }

    private void Cargar(string escena)
    {
        Time.timeScale = 1f;   // por si venías de una pausa
        SceneManager.LoadScene(escena);
    }
}