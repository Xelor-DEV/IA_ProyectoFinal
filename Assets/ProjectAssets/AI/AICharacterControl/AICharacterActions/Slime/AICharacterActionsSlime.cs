using UnityEngine;

public class AICharacterActionsSlime : AICharacterAction
{
    [SerializeField] private float belly;

    public float Belly
    {
        get
        {
            return belly;
        }
        set
        {
            belly = value;
        }
    }
}
