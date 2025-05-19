using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Move")]
public class ActionNodeWander : ActionNodeVehicle
{
    public override void OnStart()
    {
        base.OnStart();
    }

    public override TaskStatus OnUpdate()
    {
        if (healthManager.IsDead)
            return TaskStatus.Failure;

        SwitchAgentType();

        return TaskStatus.Success;
    }

    void SwitchAgentType()
    {
        switch (healthManager.AgentType)
        {
            case AgentType.Slime:
                (aiCharacterVehicle as AICharacterVehiclesSlime)?.MoveToWander();
                break;

            case AgentType.Tarr:
                (aiCharacterVehicle as AICharacterVehiclesTarr)?.MoveToWander();
                break;

            case AgentType.None:
            default:
                break;
        }
  
    }
}
