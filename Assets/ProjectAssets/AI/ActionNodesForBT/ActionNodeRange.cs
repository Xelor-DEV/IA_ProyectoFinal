using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Range")]
public class ActionNodeRange : ActionNode
{
    protected AIEyeBase aiEyeBase;
    public override void OnStart()
    {
        base.OnStart();
        aiEyeBase = gameObject.GetComponent<AIEyeBase>();
    }
}
