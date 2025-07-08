using UnityEngine;

public class AIEyeTarrShortRange : AIEyeShortRange
{
    private void Start()
    {
        LoadComponents();
    }

    private void Update()
    {
        UpdateScan();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
    }

    public override void UpdateScan()
    {
        base.UpdateScan();
    }

    private void OnValidate()
    {
        mainVision.Initialize();
        shortAttackDataView.Initialize();
    }
    private void OnDrawGizmos()
    {
        mainVision.OnDrawGizmos();
        shortAttackDataView.OnDrawGizmos();
    }
}
