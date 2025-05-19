using UnityEngine;
using UnityEngine.AI;

public class AICharacterVehicle : AICharacterControl
{
    [Header("Wander Settings")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderJitter = 1f;
    [SerializeField] private float sampleRange = 1f;

    [Header("Evade/Pursuit Settings")]
    [SerializeField] private float evadeDistance = 10f;
    [SerializeField] private float predictionTime = 2f;

    private Vector3 wanderTarget;

    public void MoveToWander()
    {
        if (AIEye.DetectedEnemy != null) return;

        // Generar punto aleatorio en el NavMesh
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderRadius;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPoint, out hit, sampleRange, NavMesh.AllAreas))
        {
            Agent.SetDestination(hit.position);
        }
    }

    public void Evade(HealthManager target)
    {
        if (target == null) return;

        Vector3 futurePosition = CalculateFuturePosition(target);
        Vector3 fleeDirection = (transform.position - futurePosition).normalized;
        Vector3 fleeTarget = transform.position + fleeDirection * evadeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeTarget, out hit, sampleRange, NavMesh.AllAreas))
        {
            Agent.SetDestination(hit.position);
        }
    }

    public void Pursue(HealthManager target)
    {
        if (target == null) return;

        Vector3 futurePosition = CalculateFuturePosition(target);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(futurePosition, out hit, sampleRange, NavMesh.AllAreas))
        {
            Agent.SetDestination(hit.position);
        }
    }

    private Vector3 CalculateFuturePosition(HealthManager target)
    {
        NavMeshAgent targetAgent = target.GetComponent<NavMeshAgent>();
        if (targetAgent != null)
        {
            return target.transform.position + targetAgent.velocity * predictionTime;
        }
        return target.transform.position;
    }
}