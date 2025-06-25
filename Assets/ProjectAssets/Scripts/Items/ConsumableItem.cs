using UnityEngine;

public class ConsumableItem : BaseItem
{
    [SerializeField] private int bellyIncreaseAmount;

    public int BellyIncreaseAmount => bellyIncreaseAmount;
}
