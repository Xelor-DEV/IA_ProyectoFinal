using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Unity.VisualScripting;
[TaskCategory("MyAI/BaseClass")]
public class ActionNode : Action
{

    protected AICharacterVehicle _IACharacterVehiculo;
    protected AICharacterAction _IACharacterActions;
    public override void OnStart()
    {
        base.OnStart();
        _IACharacterVehiculo = GetComponent<AICharacterVehicle>();
        _IACharacterActions = GetComponent<AICharacterAction>();
    }
}