using UnityEngine;
using UnityEngine.AI;

public class SimpleNPC : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderCooldown = 3f;
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private float viewAngle = 45f; // Ángulo de visión en forma de porción de pizza

    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject slime;

    [Header("Gizmo Settings")]
    [SerializeField] private Color gizmoIdleColor = Color.green;
    [SerializeField] private Color gizmoDetectedColor = Color.red;

    [SerializeField] private LayerMask itemLayer;

    private float lastWanderTime;
    private Vector3 wanderOrigin;
    private ConsumableItem currentTarget;
    private bool isReturning;

    private void Start()
    {
        wanderOrigin = transform.position;
        lastWanderTime = -wanderCooldown;
    }

    private void Update()
    {
        if (currentTarget == null && !isReturning)
        {
            if (Time.time - lastWanderTime >= wanderCooldown)
            {
                WanderToNewPoint();
            }
            DetectItems();
        }
        else if (currentTarget != null)
        {
            HandleItemConsumption();
        }
        else if (isReturning)
        {
            HandleReturnToWander();
        }
    }

    private void WanderToNewPoint()
    {
        Vector3 randomPoint = wanderOrigin + Random.insideUnitSphere * wanderRadius;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            lastWanderTime = Time.time;
        }
    }

    private void DetectItems()
    {
        Collider[] hitColliders = Physics.OverlapSphere(slime.transform.position, detectionRadius, itemLayer);

        foreach (Collider col in hitColliders)
        {
            Vector3 directionToItem = col.transform.position - slime.transform.position;
            float angle = Vector3.Angle(directionToItem, slime.transform.forward);

            if (angle < viewAngle / 2)
            {
                ConsumableItem item = col.GetComponent<ConsumableItem>();
                if (item != null && currentTarget == null)
                {
                    currentTarget = item;
                    agent.SetDestination(currentTarget.transform.position);
                    break;
                }
            }
        }
    }

    private void HandleItemConsumption()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isReturning = true;
            currentTarget = null;
            agent.SetDestination(wanderOrigin);
        }
    }

    private void HandleReturnToWander()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isReturning = false;
            lastWanderTime = Time.time;
        }
    }

    private void OnDrawGizmos()
    {
        if (slime == null) return;

        // Cambia de color cuando detecta un item
        Gizmos.color = currentTarget != null ? gizmoDetectedColor : gizmoIdleColor;

        // Dibuja la porción de pizza (tajada de pastel)
        Vector3 center = slime.transform.position;
        Vector3 forward = slime.transform.forward;
        Vector3 leftRay = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
        Vector3 rightRay = Quaternion.Euler(0, viewAngle / 2, 0) * forward;

        // Arco exterior
        DrawWireArc(center, slime.transform.up, leftRay, viewAngle, detectionRadius);

        // Rayos laterales
        Gizmos.DrawLine(center, center + leftRay * detectionRadius);
        Gizmos.DrawLine(center, center + rightRay * detectionRadius);
    }

    private void DrawWireArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
    {
        int segments = 20;
        Vector3[] points = new Vector3[segments + 1];
        Quaternion rotation = Quaternion.AngleAxis(angle / segments, normal);
        Vector3 current = from * radius;

        for (int i = 0; i <= segments; i++)
        {
            points[i] = center + current;
            current = rotation * current;
        }

        for (int i = 0; i < segments; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }
}