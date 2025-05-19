using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum FactionType 
{ 
    A, 
    B, 
    C, 
    D, 
    E 
}
public enum AgentType 
{ 
    Slime, 
    Tarr, 
    None
}

public class HealthManager : MonoBehaviour
{
    #region Health Settings
    [Header("Health Configuration")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool isImmortal = false;
    public bool IsDead => currentHealth <= 0;
    #endregion

    #region Faction & Alliance
    [Header("Faction Settings")]
    [SerializeField] private FactionType faction;
    [SerializeField] private List<FactionType> allyFactions = new List<FactionType>();
    #endregion

    #region Agent Properties
    [Header("Agent Type")]
    [SerializeField] private AgentType agentType;
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
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Transform aimOffset;


    #endregion

    #region Exposed Properties
    public FactionType Faction => faction;
    public AgentType AgentType => agentType;
    public List<FactionType> AlliedFactions => allyFactions;
    public bool IsVisible => !isInvisible;
    public Transform AimOffset
    {
        get
        {
            return aimOffset;
        }
    }
    #endregion

    #region Combat Tracking
    public HealthManager CurrentAttacker => currentAttacker;
    #endregion

    #region UI Components
    public Image HealthBarImage => healthBarImage;
    #endregion

    #region Core Functionality
    protected virtual void Awake()
    {
        InitializeHealth();
        LoadComponents();
    }

    public virtual void TakeDamage(int damage, HealthManager attacker)
    {
        if (ShouldIgnoreDamage(attacker)) return;

        ApplyDamage(damage);
        UpdateHealthDisplay();
        TrackAttacker(attacker);

        if (IsDead) HandleDeath();
    }

    private bool ShouldIgnoreDamage(HealthManager attacker)
    {
        return isImmortal || IsDead || IsAlly(attacker);
    }

    private bool IsAlly(HealthManager other)
    {
        return other != null && allyFactions.Contains(other.faction);
    }

    private void ApplyDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
    }

    private void TrackAttacker(HealthManager attacker)
    {
        if (attacker == null) return;

        if (attackerTrackingRoutine != null)
            StopCoroutine(attackerTrackingRoutine);

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
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    protected virtual void HandleDeath()
    {
        Destroy(gameObject);
    }

    public void InitializeHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthDisplay();
    }

    protected virtual void LoadComponents()
    {

    }
    #endregion

    #region Utility Methods
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthDisplay();
    }

    public void ToggleImmortality(bool state)
    {
        isImmortal = state;
    }
    #endregion
}