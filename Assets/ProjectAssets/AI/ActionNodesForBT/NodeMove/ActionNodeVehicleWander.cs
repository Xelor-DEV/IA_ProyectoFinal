using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Move")]
public class ActionNodeVehicleWander : ActionNodeVehicle
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

        SwitchAgentType();

        return TaskStatus.Success;
    }

    private void SwitchAgentType()
    {
        switch (healthManager.EntityType)
        {
            case EntityType.PinkSlime:
                (aiCharacterVehicle as AICharacterVehiclesSlime)?.Wander();
                break;

            case EntityType.Tarr:
                (aiCharacterVehicle as AICharacterVehiclesTarr)?.Wander();
                break;

            case EntityType.None:
            default:
                break;
        }
    }
}
