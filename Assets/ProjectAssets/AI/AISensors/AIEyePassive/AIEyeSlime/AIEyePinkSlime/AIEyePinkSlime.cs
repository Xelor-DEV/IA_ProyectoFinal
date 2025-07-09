using UnityEngine;

public class AIEyePinkSlime : AIEyeSlime
{
    protected override void Awake()
    {
        LoadComponents();
    }
    protected override void LoadComponents()
    {
        base.LoadComponents();
    }
    protected override void OnValidate()
    {
        mainVision.Initialize();
    }
    protected override void OnDrawGizmos()
    {
        mainVision.OnDrawGizmos();
    }
}