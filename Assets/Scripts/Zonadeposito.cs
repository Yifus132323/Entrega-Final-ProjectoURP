using UnityEngine;

// Ponlo en un objeto hijo del Barco con un Collider en modo Is Trigger.
public class ZonaDeposito : MonoBehaviour
{
    [SerializeField] private Sistema sis;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sis.Depositar();
        }
    }
}