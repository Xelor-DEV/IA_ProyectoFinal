using UnityEngine;

public enum ItemType
{
    Consumable
}

public abstract class BaseItem : MonoBehaviour
{
    [SerializeField] protected ItemType itemType;
    [SerializeField] protected string playerTag;

    public ItemType ItemType => itemType;

    protected void LoadComponents()
    {
        // Load any necessary components here, if needed
    }

    public virtual void OnCollect(ItemEffectHandler handler)
    {
        handler.UseItem(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag)
        {
            ItemEffectHandler handler = other.GetComponent<ItemEffectHandler>();
            if (handler != null)
            {
                OnCollect(handler);
                Destroy(gameObject);
            }
        }
    }
}