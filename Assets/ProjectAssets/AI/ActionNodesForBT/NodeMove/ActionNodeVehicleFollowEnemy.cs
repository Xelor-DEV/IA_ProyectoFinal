using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Move")]
public class ActionNodeVehicleFollowEnemy: ActionNodeVehicle
{
    public override void OnStart()
    {
        base.OnStart();
    }

    public override TaskStatus OnUpdate()
    {
        if (healthManager.IsDead)
        {
            return TaskStatus.Failure;
        }

        if(aiCharacterVehicle != null)
        {
            aiCharacterVehicle.MoveToEnemy();
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}