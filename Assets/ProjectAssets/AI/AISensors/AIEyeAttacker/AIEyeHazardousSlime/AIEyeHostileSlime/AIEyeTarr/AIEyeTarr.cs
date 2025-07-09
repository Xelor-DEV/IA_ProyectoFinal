using UnityEngine;

public class AIEyeTarr : AIEyeHostileSlime
{
    protected override void Awake()
    {
        LoadComponents();
    }
    protected override void LoadComponents()
    {
        base.LoadComponents();
    }
    protected override void OnValidate()
    {
        mainVision.Initialize();
        attackRangeDataView.Initialize();
    }
    protected override void OnDrawGizmos()
    {
        mainVision.OnDrawGizmos();
        attackRangeDataView.OnDrawGizmos();
    }
    protected override void FixedUpdate()
    {
        UpdateScan();
    }
    public override void UpdateScan()
    {
        base.UpdateScan();

        switch (linkedHealth.EntityGroup)
        {
            case EntityGroup.Friendly_Slime:
                if (detectedEnemy != null)
                {
                    if (detectedEnemy is FriendlySlimeHealth enemy)
                    {
                        attackRangeDataView.IsInSight(enemy.AimOffset);
                    }
                }

                break;

            case EntityGroup.Rancher:
                if (detectedEnemy != null)
                {
                    if (detectedEnemy is RancherHealth enemy)
                    {
                        attackRangeDataView.IsInSight(enemy.AimOffset);
                    }      
                }
                break;

            default:
                break;
        }
    }
}
