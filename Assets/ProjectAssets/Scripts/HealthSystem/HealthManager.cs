using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum EntityGroup
{
    Friendly_Slime,
    Hostile_Slime, 
    Human
}

public enum EntityType 
{ 
    PinkSlime, 
    Tarr,
    RancherBot,
    None
}

public class HealthManager : MonoBehaviour
{
    #region Health Settings
    [Header("Health Configuration")]
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool isImmortal = false;
    public bool IsDead
    {
        get
        {
            return currentHealth <= 0;
        } 
    }
    #endregion

    #region Faction & Alliance
    [Header("Faction Settings")]
    [SerializeField] private EntityGroup entityGroup;
    [SerializeField] private List<EntityGroup> alliedEntityGroups = new List<EntityGroup>();
    #endregion

    #region Agent Properties
    [Header("Entity Settings")]
    [SerializeField] private EntityType entityType;
    [SerializeField] private bool isInvisible = false;
    #endregion

    #region Combat & Damage
    [Header("Combat Tracking")]
    [SerializeField] private HealthManager currentAttacker;
    private Coroutine attackerTrackingRoutine;
    private float attackerMemoryDuration = 3f;
    #endregion

    #region UI Components
    [Header("UI Elements")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Transform aimOffset;
    #endregion

    #region Exposed Properties
    public EntityGroup EntityGroup
    {
        get
        {
            return entityGroup;
        }
    }
    public EntityType EntityType
    {
        get
        {
            return entityType;
        }
    }
    public List<EntityGroup> AlliedEntityGroups
    {
        get
        {
            return alliedEntityGroups;
        }
    }

    public bool IsVisible
    {
        get
        {
            return !isInvisible;
        }
    }

    public Transform AimOffset
    {
        get
        {
            return aimOffset;
        }
    }

    public HealthManager CurrentAttacker
    {
        get
        {
            return currentAttacker;
        }
    }

    public Image HealthBar
    {
        get
        {
            return healthBar;
        }
    }
    #endregion

    #region Core Functionality
    protected virtual void Awake()
    {
        LoadComponents();
        InitializeHealth();
    }

    protected virtual void LoadComponents()
    {

    }

    public void InitializeHealth()
    {
        if (currentHealth <= 0 && maxHealth > 0)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }

        UpdateHealthDisplay();
    }

    public virtual void TakeDamage(float damage, HealthManager attacker)
    {
        if (ShouldIgnoreDamage(attacker)) return;

        if (IsDead)
        {
            HandleDeath();
        }

        ApplyDamage(damage);
        UpdateHealthDisplay();
        TrackAttacker(attacker);
    }

    private bool ShouldIgnoreDamage(HealthManager attacker)
    {
        return isImmortal || IsDead || IsAlly(attacker);
    }

    private bool IsAlly(HealthManager other)
    {
        return other != null && alliedEntityGroups.Contains(other.entityGroup);
    }

    private void ApplyDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
    }

    private void TrackAttacker(HealthManager attacker)
    {
        if (attacker == null) return;

        if (attackerTrackingRoutine != null)
        {
            StopCoroutine(attackerTrackingRoutine);
        }

        attackerTrackingRoutine = StartCoroutine(TrackAttackerCoroutine(attacker));
    }

    private IEnumerator TrackAttackerCoroutine(HealthManager attacker)
    {
        currentAttacker = attacker;
        yield return new WaitForSeconds(attackerMemoryDuration);
        currentAttacker = null;
    }

    private void UpdateHealthDisplay()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

    protected virtual void HandleDeath()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Utility Methods
    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthDisplay();
    }

    public void ToggleImmortality(bool state)
    {
        isImmortal = state;
    }
    #endregion
}