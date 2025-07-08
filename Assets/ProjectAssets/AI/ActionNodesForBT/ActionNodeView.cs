using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/View")]
public class ActionNodeView : ActionNode
{
    protected AIEyeBase aiEye;
    public override void OnStart()
    {
        base.OnStart();
        aiEye = gameObject.GetComponent<AIEyeBase>();
    }
}
