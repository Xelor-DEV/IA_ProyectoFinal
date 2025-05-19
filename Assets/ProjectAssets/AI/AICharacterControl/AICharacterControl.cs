using UnityEngine;
using UnityEngine.AI;

public class AICharacterControl : MonoBehaviour
{
    public NavMeshAgent Agent { get; protected set; }
    public HealthManager Health { get; protected set; }
    public AIEye AIEye { get; protected set; }

    protected virtual void Awake()
    {
        LoadComponent();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    public virtual void LoadComponent()
    {
        Agent = GetComponent<NavMeshAgent>();
        Health = GetComponent<HealthManager>();
        AIEye = GetComponent<AIEye>();
    }
}
