
using Bitgem.VFX.StylisedWater;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MovimientoDron : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionProperty joystickAction;
    [SerializeField] private InputActionProperty joystickArriba;
    [SerializeField] private InputActionProperty joystickAbajo;
    [SerializeField] private InputActionProperty nitro;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float AlturaSpeed = 5f;
    [SerializeField] private float nitroSpeed = 10f;
    [SerializeField] private float rotationSpeed = 180f;

    [Header("Inmersion")]
    [SerializeField] private float profundidadMax = 15f;

    [Header("Energia Nitro")]
    [SerializeField] private float duracionNitro = 3f;
    [SerializeField] private float tiempoRecarga = 5f;
    [SerializeField] private Slider barraNitro;

    [Header("Efecto Submarino")]
    [SerializeField] private Image filtroBajoAgua;

    private float energiaNitro = 1f;
    private bool usandoNitro = false;
    private bool nitroPresionado = false;
    private bool nitroAgotado = false;

    private Vector2 joystickValue;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform camara;
    [SerializeField] private WateverVolumeFloater vol;

    private float arribaValue;
    private float abajoValue;
    private float offsetY = 0f;

    private void Start()
    {
        if (barraNitro != null)
        {
            barraNitro.minValue = 0f;
            barraNitro.maxValue = 1f;
            barraNitro.value = energiaNitro;
        }
    }

    private void OnEnable()
    {
        joystickAction.action.performed += MoverDron;
        joystickAction.action.canceled += MoverDron;

        joystickArriba.action.performed += MoverArriba;
        joystickArriba.action.canceled += MoverArriba;

        joystickAbajo.action.performed += MoverAbajo;
        joystickAbajo.action.canceled += MoverAbajo;

        nitro.action.performed += Nitro;
        nitro.action.canceled += Nitro;
    }

    private void OnDisable()
    {
        joystickAction.action.performed -= MoverDron;
        joystickAction.action.canceled -= MoverDron;

        joystickArriba.action.performed -= MoverArriba;
        joystickArriba.action.canceled -= MoverArriba;

        joystickAbajo.action.performed -= MoverAbajo;
        joystickAbajo.action.canceled -= MoverAbajo;

        nitro.action.performed -= Nitro;
        nitro.action.canceled -= Nitro;

        usandoNitro = false;
        nitroPresionado = false;
    }

    private void Update()
    {
        bool estaMoviendose = joystickValue.sqrMagnitude > 0.01f;

        if (nitroPresionado && estaMoviendose && !nitroAgotado && energiaNitro > 0f)
        {
            usandoNitro = true;

            energiaNitro -= Time.deltaTime / duracionNitro;

            if (energiaNitro <= 0f)
            {
                energiaNitro = 0f;
                usandoNitro = false;
                nitroAgotado = true;
            }
        }
        else
        {
            usandoNitro = false;

            energiaNitro += Time.deltaTime / tiempoRecarga;
            energiaNitro = Mathf.Clamp01(energiaNitro);
        }

        if (barraNitro != null)
        {
            barraNitro.value = energiaNitro;
        }
        // Efecto visual al sumergirse
        if (filtroBajoAgua != null)
        {
            float profundidadActual = -offsetY;

            float intensidad = Mathf.Clamp01(profundidadActual / 4f);
            float transparencia = Mathf.Lerp(0f, 0.65f, intensidad);

            Color colorFiltro = new Color(0.02f, 0.35f, 0.65f, transparencia);

            filtroBajoAgua.color = colorFiltro;
        }
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

        float cambio = (arribaValue - abajoValue) * AlturaSpeed * Time.fixedDeltaTime;

        offsetY = Mathf.Clamp(
            offsetY + cambio,
            -profundidadMax,
            0f
        );

        float objetivoY = vol.altura + offsetY;
        float velocidadY = (objetivoY - rb.position.y) / Time.fixedDeltaTime;

        if (usandoNitro)
        {
            rb.linearVelocity = new Vector3(
                movimiento.x * nitroSpeed,
                velocidadY,
                movimiento.z * nitroSpeed
            );
        }
        else
        {
            rb.linearVelocity = new Vector3(
                movimiento.x * speed,
                velocidadY,
                movimiento.z * speed
            );
        }

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
            joystickValue = Vector2.zero;
        }
    }

    public void Nitro(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            nitroPresionado = true;
        }

        if (context.canceled)
        {
            nitroPresionado = false;
            nitroAgotado = false;
            usandoNitro = false;
        }
    }

    public void MoverArriba(InputAction.CallbackContext context)
    {
        arribaValue = context.canceled ? 0f : context.ReadValue<float>();
    }

    public void MoverAbajo(InputAction.CallbackContext context)
    {
        abajoValue = context.canceled ? 0f : context.ReadValue<float>();
    }
}
