using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Indica que este componente requiere de un righibody en el mismo GameObject y lo añade 
// automaticamente al añadir este componente
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    // Referencias
    [Header("Referencias")]
    [SerializeField] 
    private Rigidbody rb;

    // Encabezado que contendrá las variables de las opciones del coche
    [Header("Car Settings")]
    public float maxSpeed = 50f;
    public float accelerationFactor = 30f;
    public float turnFactor = 15f;
    [Range(0, 1)]
    public float minSpeedFactor = 0.05f;
    public float limitDrag = 2f;
    public float gravityMultiplier = 5f;
    public bool canMove = true;
    [Range(0, 1)]
    public float driftFactor = 0.9f;
    // Ground detection
    public Transform groundCheckCenter;
    public Vector3   groundCheckSize;
    public LayerMask groundLayers;
    public float contactSkin = 0.05f;
    public Collider[] colliders;

    // Inputs
    private float _accelerationInput = 0f;
    private float _steeringInput     = 0f; // Input de giro
    private float _breakInput        = 0f;

    private float _rotationAngle     = 0f;
    [SerializeField]
    private bool _grounded;
    private bool _partialGrounded;
    private bool _partilyGrounded;
    private float _totalAceleration;
    private float _velocityForward;

    
    void Start()
    {
        for(int i = 0; i < colliders.Length; i++) {
            colliders[i].contactOffset = contactSkin;
        }
    }
 
    void Update()
    {
        
    }

    private void FixedUpdate() {
        GroundCheck();
        ApplyEngineForce();
        ApplySteering();
        KillOrthogonalVelocity();
        ApplyGravity();
    }

    private void OnDrawGizmos() {
        // Dibujado del ground box
        if(groundCheckCenter != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(groundCheckCenter.position, groundCheckSize);
        }
    }

    /// <summary>
    /// Método que aplica la fuerza del movimiento del coche
    /// </summary>
    public void ApplyEngineForce() {
        if (!_grounded && !_partialGrounded) return;
        if (!canMove) return;
        // ¿Cuánta velocidad tenemos en el forward?
        _velocityForward = Vector3.Dot(rb.velocity, transform.forward);
        // Limitamos velocidad en forward
        if (_velocityForward > maxSpeed && _totalAceleration > 0) {
            return;
        }
        // Limitamos velocidad en backward
        if (_velocityForward < -maxSpeed * 0.5f && _totalAceleration < 0) {
            return;
        }
        

        // Aplicamos drag si no hay aceleracion, de esta manera el coche frenará
        if (_totalAceleration == 0) {
            rb.drag = Mathf.Lerp(rb.drag, limitDrag, Time.deltaTime);
        } else {
            rb.drag = 0;
        }
        // Creamos el vector de fuerza para el movimiento del coche
        // Según el input del jugador
        Vector3 engineForceVector = transform.forward * _totalAceleration *
            accelerationFactor * Time.deltaTime;
        // Aplicamos la fuerza
        rb.velocity += engineForceVector;
    }

    /// <summary>
    /// Aplica giro según el input del jugador
    /// </summary>
    private void ApplySteering() {
        if (!_grounded && !_partialGrounded) return;
        if (!canMove) return;
        // Limitamos la capacidad de giro cuando este tiene poca o ninguna velocidad
        float minSpeedBeforeAllowTurningFactor = rb.velocity.magnitude * minSpeedFactor;
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);
        // Actualizamos la rotación según el input
        int dirMultiplier = _velocityForward >= 0 ? 1 : -1;
        float deltaRotation = _steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor * dirMultiplier;
        _rotationAngle += deltaRotation;
        // Aplicamos la rotación en Y. Reseteamos el valor de la rotacion
        // para que no se sume indefinidamente con el input
        rb.angularVelocity = Vector3.zero; 
        rb.AddTorque(transform.up * deltaRotation, ForceMode.Impulse);
    }

    /// <summary>
    /// Corrige la trayectoria del movimiento del coche según su forward
    /// </summary>
    private void KillOrthogonalVelocity() {
        if (!_grounded && !_partialGrounded) return;
        // Extraemos la velocidad que lleva el coche en el forward
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(rb.velocity, transform.forward);
        // Estraemos la velocidad en right
        Vector3 rightVelocity = transform.right * Vector3.Dot(rb.velocity, transform.right);
        // Actualizamos la velocidad
        Vector3 fixedVelocity = forwardVelocity + rightVelocity * driftFactor;
        // Conservamos la velocidad en Y que ya lleva el rigidbody
        fixedVelocity.y = rb.velocity.y;
        // Reasignamos la velocidad del rigidbody
        rb.velocity = fixedVelocity;
    }

    /// <summary>
    /// Aplica la fuerza de la gravedad
    /// </summary>
    private void ApplyGravity() {
        if (_grounded) return;
        rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);

    }

    /// <summary>
    /// Detecta si el coche se encuentra en el suelo o no
    /// </summary>
    private void GroundCheck() {
        Collider[] colisions = Physics.OverlapBox(groundCheckCenter.position, groundCheckSize / 2f,
            groundCheckCenter.rotation, groundLayers);

        if(colisions != null && colisions.Length > 0) {
            _partialGrounded = true;

            Vector3 rayOffset = new Vector3(0, 0.5f, 0);
            Ray ray = new Ray(transform.position + rayOffset, Vector3.down);
            Debug.DrawRay(transform.position + rayOffset, Vector3.down, Color.red);
            if (Physics.Raycast(ray, out RaycastHit hit, 1f)) {
                float angle = Mathf.Abs(Vector3.Angle(hit.normal, transform.up));
                _grounded = angle < 1f;
            } else {
                _grounded = false;
            }
            return;
        }
        _grounded = false;
        _partialGrounded = false;
    }

    /// <summary>
    /// Gestiona la insercción del input del jugador
    /// </summary>
    /// <param name="accelerationInput"></param>
    public void SetInput(float steeringInput, float accelerationInput, float breakInput) {
        _steeringInput = steeringInput;
        _accelerationInput = accelerationInput;
        _breakInput = breakInput;
        // Calculamos la aceleración total a aplicar en base de los inputs
        _totalAceleration = _accelerationInput - _breakInput;
    }


}
