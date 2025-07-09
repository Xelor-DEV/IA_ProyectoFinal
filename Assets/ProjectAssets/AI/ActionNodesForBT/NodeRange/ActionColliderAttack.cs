using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Range")]
public class ActionColliderAttack : ActionNodeRange
{
    public override void OnAwake()
    {
        base.OnAwake();
    }
    public override TaskStatus OnUpdate()
    {
        if (healthManager.IsDead)
        {
            return TaskStatus.Failure;
        }

        if (aiEyeBase is AIEyeAttacker attacker)
        {
            if (attacker.DetectedEnemy != null && attacker.AttackRangeDataView.TargetInSight)
            {
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Failure;
    }
}