using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
[TaskCategory("MyAI/View")]
public class ActionNodeNotViewEnemy : ActionNodeView
{
    public override void OnStart()
    {
        base.OnStart();
    }
    public override TaskStatus OnUpdate()
    {
        if (aiEye.DetectedEnemy == null)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}