using Unity.VisualScripting;
using UnityEngine;

public class AICharacterVehicletest : AICharacterVehicleTest
{
    private float deviationTimer;

    protected override void Start()
    {
        base.Start();
        SetRandomDestination();
        deviationTimer = deviationUpdateInterval;
    }

    protected override void Update()
    {
        base.Update();

        if (isDestinationReached)
        {
            SetRandomDestination();
            return;
        }

        deviationTimer -= Time.deltaTime;
        if (deviationTimer <= 0)
        {
            UpdateDeviationAngle();
            deviationTimer = deviationUpdateInterval;
        }

        MoveTowardsDestination();
        CheckDestinationReached();
    }

    private void MoveTowardsDestination()
    {
        // Calcular dirección base al destino
        Vector3 direction = (currentDestination - transform.position).normalized;

        // Aplicar desvío a la dirección
        Vector3 deviatedDirection = Quaternion.Euler(0, currentDeviationAngle, 0) * direction;

        // Actualizar rotación para mirar en la dirección desviada
        transform.rotation = Quaternion.LookRotation(deviatedDirection);

        // Mover hacia adelante en la dirección actual del transform
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
    }

}