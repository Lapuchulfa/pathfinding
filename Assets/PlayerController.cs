using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f; // Qué tan rápido gira el personaje

    [Header("References")]
    public Animator animator;
    public Transform cameraTransform; // Arrastra la Cámara Principal aquí

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 moveDirection;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (controller == null)
            Debug.LogError("No se encontró un CharacterController en este GameObject.");

        if (animator == null)
            animator = GetComponent<Animator>();

        // Si no asignas la cámara en el inspector, busca la principal automáticamente
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (controller == null) return;

        CalculateMovement();
        ApplyRotation();
        UpdateControlAndAnimations();
    }

    private void CalculateMovement()
    {
        // Tomamos la dirección de la cámara para que el movimiento sea relativo a ella
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Aplanamos en el eje Y para que el jugador no intente volar o hundirse al mirar arriba/abajo
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calculamos la dirección final basada en el Input
        moveDirection = (forward * moveInput.y) + (right * moveInput.x);
    }

    private void ApplyRotation()
    {
        // Si hay movimiento, el personaje rota suavemente hacia esa dirección (estilo Fortnite/Tercera persona)
        if (moveInput.magnitude > 0.1f && moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateControlAndAnimations()
    {
        // Aplicamos el movimiento a través del CharacterController
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Control de animaciones
        if (animator != null)
        {
            // Usamos moveInput en vez de move por si la gravedad o colisiones alteran la magnitud
            bool isWalking = moveInput.magnitude > 0.1f;
            animator.SetBool("isWalking", isWalking);
        }
    }

    // Este método es llamado por el componente Player Input
   
}