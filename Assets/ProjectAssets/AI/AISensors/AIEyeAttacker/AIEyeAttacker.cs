using UnityEditor;
using UnityEngine;

public class AIEyeAttacker : AIEyeBase
{
    [SerializeField] protected DataView attackRangeDataView = new DataView();

    public DataView AttackRangeDataView
    {
        get
        {
            return attackRangeDataView;
        }
        set
        {
            attackRangeDataView = value;
        }
    }
}
