using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Range")]
public class ActionColliderNotAttack : ActionNodeRange
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
            if (!attacker.AttackRangeDataView.TargetInSight)
            {
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Failure;
    }
}
