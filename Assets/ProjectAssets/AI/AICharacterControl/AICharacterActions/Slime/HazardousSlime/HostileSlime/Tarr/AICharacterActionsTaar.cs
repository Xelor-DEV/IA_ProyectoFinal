using UnityEngine;

public class AICharacterActionsTaar : AICharacterActionsHostileSlime
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

    public void Attack()
    {

        if (FrameRate > Rate)
        {
            FrameRate = 0;
            AIEyeTarrShortRange aiEyeTarrShortRange = ((AIEyeTarrShortRange)aiEye);

            if (aiEyeTarrShortRange != null && aiEyeTarrShortRange.DetectedEnemy != null)
            {
                aiEyeTarrShortRange.DetectedEnemy.TakeDamage(damage, health);
            }

        }

        FrameRate += Time.deltaTime;
    }
}
