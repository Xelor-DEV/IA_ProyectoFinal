using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Move")]
public class ActionNodeVehicleEvade : ActionNodeVehicle
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

        if (aiCharacterVehicle != null)
        {
            aiCharacterVehicle.Evade();
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}