using TMPro;
using UnityEngine;

public class Sistema : MonoBehaviour
{
    public TextMeshProUGUI textoPuntuacion;
    public int puntos;

    private void Update()
    {
        textoPuntuacion.text = puntos.ToString();
    }
}
