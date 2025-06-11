using UnityEngine;

public class FuzzyLogicVehicles : MonoBehaviour
{
    [SerializeField] private CalculateFuzzy angleDependDistanceEnemy;

    public CalculateFuzzy AngleDependDistanceEnemy
    {
        get
        {
            return angleDependDistanceEnemy;
        }
        set
        {
            angleDependDistanceEnemy = value;
        }
    }
}