using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Action")]
public class ActionNodeActionAttack : ActionNodeAction
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

        if (aiCharacterAction is AICharacterActionsHazardousSlime slime)
        {
            if  (aiCharacterAction is AICharacterActionsTarr tarr)
            {
                tarr.Attack();
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Failure;
    }
}