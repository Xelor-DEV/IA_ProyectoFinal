using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Unity.VisualScripting;
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