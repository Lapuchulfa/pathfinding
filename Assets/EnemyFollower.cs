using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyFollower : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Configuración de Detección")]
    public float detectionRange = 10f;

    [Header("Configuración Visual (Gizmos)")]
    public bool drawGizmo = true;
    public Color gizmoColor = Color.cyan;
    public Color gizmoDetectColor = Color.red;

    private bool isPlayerDetected = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        HandleDetectionAndMovement();
        UpdateAnimations();
    }

    private void HandleDetectionAndMovement()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= detectionRange)
        {
            isPlayerDetected = true;
            agent.SetDestination(player.position);
        }
        else
        {
            isPlayerDetected = false;
            agent.ResetPath();
        }
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        // Evaluamos si el agente se está moviendo realmente
        bool isMoving = agent.velocity.sqrMagnitude > 0.1f;

        // Si se mueve, la animación corre a velocidad normal (1). Si se detiene, la congelamos (0).
        animator.speed = isMoving ? 1f : 0f;
    }

    // --- DIBUJAR EL RANGO EN EL EDITOR ---
    void OnDrawGizmos()
    {
        if (!drawGizmo) return;

        if (Application.isPlaying)
        {
            Gizmos.color = isPlayerDetected ? gizmoDetectColor : gizmoColor;
        }
        else
        {
            Gizmos.color = gizmoColor;
        }

        DrawCircleGizmo(transform.position, detectionRange);
    }

    private void DrawCircleGizmo(Vector3 center, float radius)
    {
        int segments = 64;
        float angleStep = 360f / segments;
        Vector3 lastPoint = Vector3.zero;

        Matrix4x4 currentMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.identity, Vector3.one);

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;
            Vector3 currentPoint = new Vector3(x, 0, z);

            if (i > 0)
            {
                Gizmos.DrawLine(lastPoint, currentPoint);
            }
            lastPoint = currentPoint;
        }

        Gizmos.matrix = currentMatrix;
    }
}