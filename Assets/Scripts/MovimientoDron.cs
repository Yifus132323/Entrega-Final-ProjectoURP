using Bitgem.VFX.StylisedWater;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoDron : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionProperty joystickAction;
    [SerializeField] private InputActionProperty nitro;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float nitroSpeed = 10f;
    [SerializeField] private float rotationSpeed = 180f;

    private Vector2 joystickValue;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform camara;
    [SerializeField] private WateverVolumeFloater vol;
    [SerializeField] private bool usandoNitro = false;

    private void OnEnable()
    {
        joystickAction.action.performed += MoverDron;
        joystickAction.action.canceled += MoverDron;
        nitro.action.performed += Nitro;
        nitro.action.canceled += Nitro;
    }

    private void OnDisable()
    {
        joystickAction.action.performed -= MoverDron;
        joystickAction.action.canceled -= MoverDron;
        nitro.action.performed -= Nitro;
        nitro.action.canceled -= Nitro;
    }

    private void FixedUpdate()
    {
        Vector3 adelante = camara.forward;
        Vector3 derecha = camara.right;

        adelante.y = 0f;
        derecha.y = 0f;

        adelante.Normalize();
        derecha.Normalize();

        Vector3 movimiento = adelante * joystickValue.y + derecha * joystickValue.x;

        // Velocidad horizontal controlada por ti, vertical para seguir la altura del agua
        float velocidadY = (vol.altura - rb.position.y) / Time.fixedDeltaTime;
        if (usandoNitro)
        {
            rb.linearVelocity = new Vector3(movimiento.x * nitroSpeed, velocidadY, movimiento.z * nitroSpeed);
        }
        else
        {
            rb.linearVelocity = new Vector3(movimiento.x * speed, velocidadY, movimiento.z * speed);
        }

        // Descarta cualquier giro que haya metido una colisión
        rb.angularVelocity = Vector3.zero;

        if (movimiento.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movimiento);
            Quaternion newRotation = Quaternion.RotateTowards(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
            rb.MoveRotation(newRotation);
        }
    }

    public void MoverDron(InputAction.CallbackContext context)
    {
        joystickValue = context.ReadValue<Vector2>();
        if (context.canceled)
        {
            joystickValue = new Vector2(0, 0);
        }
        Debug.Log(joystickValue);
    }

    public void Nitro(InputAction.CallbackContext context)
    {
        Debug.Log("Usando Nitro");
        usandoNitro = true;
        if (context.canceled)
        {
            usandoNitro = false;
            Debug.Log("Nitro soltado");
        }
    }

}
