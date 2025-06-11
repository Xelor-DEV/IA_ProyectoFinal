using UnityEngine;

public class AICharacterVehicleTest : AICharacterVehicle
{
    [Header("Fuzzy Settings")]
    [SerializeField] private FuzzyLogicVehicles fuzzyLogic;
    [SerializeField] private float maxDeviationAngle = 30f;
    [SerializeField] protected float movementSpeed = 5f;
    [SerializeField] private float destinationThreshold = 0.5f;
    [SerializeField] protected float areaRadius = 20f;
    [SerializeField] protected float deviationUpdateInterval = 0.5f;

    protected Vector3 currentDestination;
    protected bool isDestinationReached = true;
    protected float currentDeviationAngle;





    protected void SetRandomDestination()
    {
        Vector2 randomCircle = Random.insideUnitCircle * areaRadius;
        currentDestination = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        isDestinationReached = false;

        // Visualización en consola
        Debug.Log($"Nuevo destino establecido: {currentDestination}");
    }

    protected void UpdateDeviationAngle()
    {
        float distance = Vector3.Distance(transform.position, currentDestination);
        float deviationFactor = fuzzyLogic.AngleDependDistanceEnemy.CalculateFuzzyValue(distance);
        currentDeviationAngle = Random.Range(-1f, 1f) * deviationFactor * maxDeviationAngle;

        // Visualización en consola
        Debug.Log($"Distancia: {distance:F1}m | Desvío: {currentDeviationAngle:F1}°");
    }


    protected void CheckDestinationReached()
    {
        float distance = Vector3.Distance(transform.position, currentDestination);
        if (distance <= destinationThreshold)
        {
            isDestinationReached = true;
            Debug.Log("Destino alcanzado!");
        }
    }

    private void OnDrawGizmos()
    {
        // Dibujar destino
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(currentDestination, 0.3f);

        // Dibujar línea al destino
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, currentDestination);

        // Dibujar dirección actual
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
    }
}