using UnityEngine;
using UnityEngine.AI;

public class AICharacterVehicle : AICharacterControl
{
    [Header("Movement Settings")]
    [SerializeField] protected float rotationSpeed = 50f; // Speed of rotation towards target position
    [SerializeField] protected float positionSampleRadius = 1f; // Radius to check for valid NavMesh positions
    [SerializeField] protected float defaultSpeed = 3.5f;
    [SerializeField] protected float maxSpeed = 5f;

    [Header("Wander Settings")]
    [SerializeField] protected float wanderRange = 10f; // Maximum distance for wander points
    [SerializeField] protected float wanderInterval = 4f; // Time between new wander point selections

    [Header("Move To Enemy")]
    [SerializeField] protected float thresholdDistance = 0.3f;

    [Header("Evade Settings")]
    [SerializeField] protected float evadeDistance = 10f;
    [SerializeField] protected float evadeSpeedMultiplier = 1.5f;
    [SerializeField] protected float evadeCooldown = 5f;
    protected float _lastEvadeTime = -10f;

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

    protected virtual void LookPosition(Vector3 position)
    {
        Vector3 dir = (position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotationSpeed);
    }

    public virtual void MoveToEnemy()
    {
        if (aiEye != null && aiEye.DetectedEnemy != null)
        {
            agent.stoppingDistance = ((AIEyeAttacker)aiEye).AttackRangeDataView.MaxDistance - thresholdDistance;

            agent.speed = maxSpeed;

            LookPosition(aiEye.DetectedEnemy.transform.position);

            MoveToPosition(aiEye.DetectedEnemy.transform.position);
        }
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
        agent.speed = defaultSpeed;

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

    public virtual void Evade()
    {
        // Verificar cooldown
        if (Time.time - _lastEvadeTime < evadeCooldown)
        {
            return;
        }

        if (aiEye != null && aiEye.DetectedEnemy != null)
        {
            Vector3 evadeDirection = (transform.position - aiEye.DetectedEnemy.transform.position).normalized;

            Vector3 evadeTarget = transform.position + evadeDirection * evadeDistance;

            agent.speed = maxSpeed * evadeSpeedMultiplier;
            agent.stoppingDistance = 0f;

            MoveToPosition(evadeTarget);

            _lastEvadeTime = Time.time;

            Debug.Log($"Evadiendo de {aiEye.DetectedEnemy.gameObject.name}");
        }
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