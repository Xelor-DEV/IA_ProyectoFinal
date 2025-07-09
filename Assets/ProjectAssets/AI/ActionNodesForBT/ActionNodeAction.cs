using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/BaseClass")]
public class ActionNodeAction : ActionNode
{
    protected AICharacterAction aiCharacterAction;
    public override void OnStart()
    {
        base.OnStart();
        aiCharacterAction = gameObject.GetComponent<AICharacterAction>();
    }
}
