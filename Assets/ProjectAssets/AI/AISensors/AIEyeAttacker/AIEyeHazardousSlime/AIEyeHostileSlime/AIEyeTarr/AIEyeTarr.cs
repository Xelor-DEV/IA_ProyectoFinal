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

        if (detectedEnemy == null)
        {
            attackRangeDataView.TargetInSight = false;
            return;
        }

        switch (linkedHealth.EntityGroup)
        {
            case EntityGroup.Hostile_Slime:
                if (detectedEnemy != null)
                {
                    if (detectedEnemy is FriendlySlimeHealth enemySlime)
                    {
                        attackRangeDataView.IsInSight(enemySlime.AimOffset);
                    }

                    if (detectedEnemy is RancherHealth enemyRancher)
                    {
                        attackRangeDataView.IsInSight(enemyRancher.AimOffset);
                    }
                }
                break;

            default:
                break;
        }
    }
}
