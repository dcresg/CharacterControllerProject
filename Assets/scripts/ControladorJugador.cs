using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorJugador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform Pivote;
    [SerializeField] private Camera camara;
    [SerializeField] private GameObject modelo;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5;
    [SerializeField] private float alturaSalto = 1.5f;
    [SerializeField] private float gravedad = -20f;
    [SerializeField] private float sensibilidadMouse = 0.12f;

    [Header("Camara")]
    [SerializeField] private Vector3 posicionTercera = new Vector3(0.65f, 0.15f, -3f);
    [SerializeField] private Vector3 posicionApuntado = new Vector3(0.55f, 0.1f, -1.7f);
    [SerializeField] private float suavizado = 10f;

    private bool primeraPersona = false;

    private float velocidadVertical;
    private float inclinacion = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private CharacterController controlador;
    private PlayerInput input;
    private InputAction mover;
    private InputAction saltar;
    private InputAction mirar;
    private InputAction apuntar;
    private InputAction cambiarvista;
    void Start()
    {
        controlador = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();

        mover = input.actions["Move"];
        saltar = input.actions["Jump"];
        mirar = input.actions["Look"];
        apuntar = input.actions["Point"];
        cambiarvista = input.actions["ChangeView"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (cambiarvista.WasPressedThisFrame()) 
        {
            primeraPersona = !primeraPersona;
            modelo.SetActive(!primeraPersona);
        }
        GirarCamara();
        MoverPersonaje();
        ActualizarCamara();
    }

    void GirarCamara() 
    {
        Vector3 deltaMouse = mirar.ReadValue<Vector2>();

        Debug.Log(deltaMouse);

        transform.Rotate(Vector3.up * deltaMouse.x * sensibilidadMouse);

        inclinacion -= deltaMouse.y;

        inclinacion = Mathf.Clamp(inclinacion, -80f, 80f);

        Pivote.localRotation = Quaternion.Euler(inclinacion, 0f, 0f);


    }

    void MoverPersonaje()
    {
        Vector2 entrada = mover.ReadValue<Vector2>();

        Vector3 direccion = transform.right * entrada.x + transform.forward * entrada.y;

        direccion = Vector3.ClampMagnitude(direccion, 1f);

        if(controlador.isGrounded && saltar.WasPressedThisFrame()) 
        {
            velocidadVertical = Mathf.Sqrt(alturaSalto * -2f * gravedad);
        }

        velocidadVertical += gravedad * Time.deltaTime;

        Vector3 desplazamiento = direccion * velocidad;

        desplazamiento.y = velocidadVertical;

        controlador.Move(desplazamiento * Time.deltaTime);
    }

    void ActualizarCamara()
    {
        Vector3 destino;

        if (primeraPersona)
        {
            destino = Vector3.zero;
        }

        else 
        {
            destino = posicionTercera;
        }

        camara.transform.localPosition = Vector3.Lerp(camara.transform.localPosition, destino, suavizado * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "cubo") 
        {
            Destroy(hit.gameObject);
        }
    }
}
