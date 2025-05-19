using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
[TaskCategory("MyAI/View")]
public class ActionNodeViewSlime : ActionNodeView
{
    public override void OnStart()
    {
        base.OnStart();
    }
    public override TaskStatus OnUpdate()
    {
        if (aiEye.DetectedEnemy != null &&
           aiEye.DetectedEnemy.AgentType == AgentType.Slime)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
