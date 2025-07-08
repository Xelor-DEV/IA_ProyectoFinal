using UnityEngine;
using UnityEngine.AI;

public class AICharacterControl : MonoBehaviour
{
    [Header("Debug Show Components")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected HealthManager health;
    [SerializeField] protected AIEye aiEye;

    public NavMeshAgent Agent
    {
        get
        {
            return agent;
        }
        protected set
        {
            agent = value;
        }
    }

    public HealthManager Health 
    {
        get
        {
            return health;
        }
        protected set
        {
            health = value;
        }
    }

    public AIEye AIEye 
    {
        get
        {
            return aiEye;
        }
        protected set
        {
            aiEye = value;
        }
    }

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

    protected virtual void LoadComponent()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<HealthManager>();
        aiEye = GetComponent<AIEye>();
    }
}
