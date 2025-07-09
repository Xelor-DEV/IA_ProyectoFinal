using UnityEngine;

public class AICharacterActionsTarr : AICharacterActionsHostileSlime
{
    public float FrameRate = 0;
    public float Rate = 1;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
    }

    public override void Attack()
    {
        if (FrameRate > Rate)
        {
            FrameRate = 0;

            AIEyeAttacker aiEyeTarrShortRange = ((AIEyeAttacker)aiEye);
            if (aiEyeTarrShortRange != null &&
                aiEyeTarrShortRange.DetectedEnemy != null)
            {
                aiEyeTarrShortRange.DetectedEnemy.TakeDamage(damage, health);
            }
        }

        FrameRate += Time.deltaTime;
    }
}
