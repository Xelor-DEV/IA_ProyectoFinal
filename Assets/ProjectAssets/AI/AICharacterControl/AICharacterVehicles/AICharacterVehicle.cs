using UnityEngine;
using UnityEngine.AI;

public class AICharacterVehicle : AICharacterControl
{
    [Header("Movement Settings")]
    [SerializeField] protected float rotationSpeed = 50f; // Speed of rotation towards target position
    [SerializeField] protected float positionSampleRadius = 1f; // Radius to check for valid NavMesh positions

    [Header("Wander Settings")]
    [SerializeField] protected float wanderRange = 10f; // Maximum distance for wander points
    [SerializeField] protected float wanderInterval = 4f; // Time between new wander point selections

    [Header("Gizmos Settings")]
    [SerializeField] protected bool showGizmos = true;
    [SerializeField] protected Color wanderRangeColor = Color.cyan;
    [SerializeField] protected Color wanderTargetColor = Color.yellow;
    [SerializeField] protected Color validPositionColor = Color.green;
    [SerializeField] protected Color invalidPositionColor = Color.red;

    protected Vector3 currentWanderTarget; // Current wander destination
    protected float wanderTimer; // Timer for wander point updates
    protected bool lastPositionValid; // Track if last sampled position was valid

    protected override void Awake()
    {
        base.Awake();
        currentWanderTarget = GetRandomWanderPosition(transform.position, wanderRange);
    }

    protected virtual void LookTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    protected virtual void MoveToPosition(Vector3 targetPosition)
    {
        // Sample position on NavMesh to ensure accessibility
        if (SampleValidPosition(targetPosition, positionSampleRadius, out Vector3 validPosition))
        {
            agent.SetDestination(validPosition);
            lastPositionValid = true;
        }
        else
        {
            // Handle inaccessible position (optional: search for nearest valid point)
            lastPositionValid = false;
            Debug.LogWarning("Target position not accessible on NavMesh: " + targetPosition);
        }
    }

    protected bool SampleValidPosition(Vector3 targetPosition, float sampleRadius, out Vector3 result)
    {
        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = targetPosition;
        return false;
    }

    protected Vector3 GetRandomWanderPosition(Vector3 origin, float range)
    {
        Vector3 randomPosition = origin + Random.insideUnitSphere * range;
        randomPosition.y = origin.y; // Maintain same height

        // Ensure position is valid on NavMesh
        if (SampleValidPosition(randomPosition, positionSampleRadius, out Vector3 validPosition))
        {
            return validPosition;
        }
        return origin; // Fallback to origin if no valid position found
    }

    public virtual void Wander()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentWanderTarget);

        // Get new wander position if close to target or interval passed
        if (distanceToTarget < 2f || wanderTimer > wanderInterval)
        {
            currentWanderTarget = GetRandomWanderPosition(transform.position, wanderRange);
            wanderTimer = 0f;
        }

        wanderTimer += Time.deltaTime;
        MoveToPosition(currentWanderTarget);
    }

    protected virtual void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // Draw wander range
        Gizmos.color = wanderRangeColor;
        Gizmos.DrawWireSphere(transform.position, wanderRange);

        // Draw position sample radius
        Gizmos.color = lastPositionValid ? validPositionColor : invalidPositionColor;
        Gizmos.DrawWireSphere(currentWanderTarget, positionSampleRadius);

        // Draw current wander target
        Gizmos.color = wanderTargetColor;
        Gizmos.DrawSphere(currentWanderTarget, 0.3f);
        Gizmos.DrawLine(transform.position, currentWanderTarget);
    }
}