using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
[TaskCategory("MyAI/BaseClass")]
public class ActionNodeVehicle : ActionNode
{
    protected AICharacterVehicle aiCharacterVehicle;
    public override void OnStart()
    {
        base.OnStart();
        aiCharacterVehicle = GetComponent<AICharacterVehicle>();
    }
}
