using UnityEngine;

public class AIEyeShortRange : AIEyeAttack
{
    [SerializeField] protected DataView shortAttackDataView = new DataView();

    protected override void LoadComponents()
    {
        base.LoadComponents();
    }

    public override void UpdateScan()
    {
        base.UpdateScan();
        if(detectedEnemy != null)
        {
            shortAttackDataView.IsInSight(detectedEnemy.AimOffset);
        }
        else
        {
            shortAttackDataView.TargetInSight = false;
            mainVision.TargetInSight = false;
        }
    }
}
