using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
[TaskCategory("MyAI/View")]
public class ActionNodeNotViewTarr : ActionNodeView
{
    public override void OnStart()
    {
        base.OnStart();
    }

    public override TaskStatus OnUpdate()
    {
        // Devuelve Success cuando NO es un Tarr pero hay un enemigo detectado
        if (aiEye.DetectedEnemy != null &&
           aiEye.DetectedEnemy.AgentType != AgentType.Tarr)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
