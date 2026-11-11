using UnityEngine;

public class SeguirJugador : MonoBehaviour
{
    [SerializeField] private Transform Dron;
    [SerializeField] private float distancia = 8f;
    [SerializeField] private float alturaOffset = 1f;   // NUEVO: altura de la cámara respecto al dron

    void Update()
    {
        Vector3 direccion = Dron.forward;

        Vector3 nuevaPosicion = Dron.position - direccion * distancia;
        nuevaPosicion.y = Dron.position.y + alturaOffset;   // CAMBIO: antes era transform.position.y (altura fija)

        transform.position = Vector3.Lerp(
            transform.position,
            nuevaPosicion,
            5f * Time.deltaTime
        );

        Quaternion rotacionObjetivo = Quaternion.LookRotation(
            Dron.position - transform.position
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotacionObjetivo,
            5f * Time.deltaTime
        );
    }
}