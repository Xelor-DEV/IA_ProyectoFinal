using UnityEngine;

public class ItemEffectHandler : MonoBehaviour
{
    [SerializeField] private AICharacterActionsSlime actionsSlime;
    public void UseItem(BaseItem item)
    {
        switch (item.ItemType)
        {
            case ItemType.Consumable:
                if (item is ConsumableItem consumableItem)
                {
                    actionsSlime.Belly += consumableItem.BellyIncreaseAmount;
                }
                break;
        }
    }
}