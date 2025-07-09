using UnityEngine;

public class AICharacterActionsHazardousSlime : AICharacterActionsSlime, IAttackerCharacterAction
{
    [Header("Damage Settings")]
    [SerializeField] protected float damage;

    public virtual void Attack()
    {

    }
}
