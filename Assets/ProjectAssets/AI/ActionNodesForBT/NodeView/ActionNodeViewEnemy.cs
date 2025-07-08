using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
[TaskCategory("MyAI/View")]
public class ActionNodeViewEnemy : ActionNodeView
{
    public override void OnStart()
    {
        base.OnStart();
    }
    public override TaskStatus OnUpdate()
    {
        if (aiEye.DetectedEnemy == null)
        {
            return TaskStatus.Failure;
        }

        return TaskStatus.Success;
    }
}