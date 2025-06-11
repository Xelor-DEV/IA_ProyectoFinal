using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
[TaskCategory("MyAI/View")]
public class ActionNodeView : ActionNode
{
    protected AIEye aiEye;
    public override void OnStart()
    {
        base.OnStart();
        aiEye = gameObject.GetComponent<AIEye>();
    }
}
