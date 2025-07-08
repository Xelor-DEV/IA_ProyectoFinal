using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/BaseClass")]
public class ActionNodeVehicle : ActionNode
{
    protected AICharacterVehicle aiCharacterVehicle;
    public override void OnStart()
    {
        base.OnStart();
        aiCharacterVehicle = GetComponent<AICharacterVehicle>();
    }
}
