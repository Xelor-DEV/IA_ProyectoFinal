using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/BaseClass")]
public class ActionNode : Action
{
    protected HealthManager healthManager;

    public override void OnStart()
    {
        base.OnStart();
        healthManager = gameObject.GetComponent<HealthManager>();
    }
}