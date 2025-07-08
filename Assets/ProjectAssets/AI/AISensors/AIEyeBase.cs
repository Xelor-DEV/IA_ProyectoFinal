using UnityEngine;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine.Playables;

public class DataViewBase
{
    #region Vision Configuration
    [Header("Vision Range Settings")]
    [SerializeField, Range(0, 180)] private float visionAngle = 30f;
    [SerializeField] private float visionHeight = 1.0f;
    [SerializeField] private float maxDetectionDistance = 10f;
    [SerializeField] private LayerMask scanLayers;

    [Header("Visualization Settings")]
    [SerializeField] protected Color outOfSightColor = Color.red;
    [SerializeField] protected bool drawGizmos = false;

    [Header("Dependencies")]
    [SerializeField] private HealthManager owner;
    #endregion

    #region Properties
    public float VisionAngle => visionAngle;
    public float VisionHeight => visionHeight;
    public float MaxDistance => maxDetectionDistance;
    public LayerMask ScanLayers => scanLayers;
    public HealthManager Owner
    {
        get
        {
            return owner;
        }
        set
        {
            owner = value;
        }
    }
    public Mesh VisionMesh { get; protected set; }

    public DataViewBase()
    {

    }
    #endregion

    #region Core Functionality
    public virtual void Initialize()
    {
        VisionMesh = CreateWedgeMesh();
    }

    public virtual bool IsInSight(Transform target)
    {
        return false;
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();
        int segments = 10;
        int numTriangles = (segments * 4) + 4;
        int numVertices = numTriangles * 3;
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -visionAngle, 0) * Vector3.forward * maxDetectionDistance;
        Vector3 bottomRight = Quaternion.Euler(0, visionAngle, 0) * Vector3.forward * maxDetectionDistance;

        Vector3 topCenter = bottomCenter + Vector3.up * visionHeight;
        Vector3 topLeft = bottomLeft + Vector3.up * visionHeight;
        Vector3 topRight = bottomRight + Vector3.up * visionHeight;

        int vert = 0;

        // left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -visionAngle;
        float deltaAngle = (visionAngle * 2) / segments;
        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * maxDetectionDistance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * maxDetectionDistance;

            topRight = bottomRight + Vector3.up * visionHeight;
            topLeft = bottomLeft + Vector3.up * visionHeight;

            // far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;
            // top 
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;
            // bottom 
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;

        }


        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;

    }
    #endregion

    #region Gizmos
    public virtual void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        if (VisionMesh != null && Owner != null)
        {
            Gizmos.color = outOfSightColor;
            Gizmos.DrawMesh(VisionMesh, Owner.transform.position, Owner.transform.rotation);
        }
    }
    #endregion
}

[Serializable]
public class DataView : DataViewBase
{
    #region Occlusion Settings
    [Header("Occlusion Settings")]
    [SerializeField] private LayerMask occlusionLayers;
    [SerializeField] private bool checkInsideObjects = true;
    [SerializeField] private Color inSightColor = Color.green;
    #endregion

    #region Vision State
    public bool TargetInSight { get; set; }
    #endregion

    #region Detection Logic
    public override bool IsInSight(Transform target)
    {
        TargetInSight = false;
        if (target == null || Owner == null) return false;

        Vector3 origin = Owner.AimOffset.position;
        Vector3 targetPosition = target.position;
        Vector3 direction = targetPosition - origin;

        if (!ValidateDistance(direction) || !ValidateHeight(targetPosition) || !ValidateAngle(direction))
            return TargetInSight;

        if (CheckOcclusion(origin, targetPosition)) return TargetInSight;

        TargetInSight = true;
        return TargetInSight;
    }

    private bool ValidateDistance(Vector3 direction)
    {
        return direction.magnitude <= MaxDistance;
    }

    private bool ValidateHeight(Vector3 targetPos)
    {
        return Mathf.Abs(targetPos.y - Owner.transform.position.y) <= VisionHeight;
    }
        
    private bool ValidateAngle(Vector3 direction)
    {
        float horizontalAngle = Vector3.Angle(direction.normalized, Owner.transform.forward);
        return horizontalAngle <= VisionAngle;
    }

    private bool CheckOcclusion(Vector3 origin, Vector3 target)
    {
        return Physics.Linecast(origin, target, occlusionLayers) && checkInsideObjects;
    }
    #endregion

    #region Gizmos
    public override void OnDrawGizmos()
    {
        if (!drawGizmos) return;


        if (VisionMesh != null && Owner != null)
        {
            if (TargetInSight)
            {
                Gizmos.color = inSightColor;
            }
            else
                Gizmos.color = inSightColor;

            Gizmos.DrawMesh(VisionMesh, Owner.transform.position, Owner.transform.rotation);
        }
    }
    #endregion
}

public class AIEyeBase : MonoBehaviour
{
    #region Scan Settings
    [Header("Scan Configuration")]
    [SerializeField] protected DataView mainVision = new DataView();
    [SerializeField] protected RandomBuffer scanIntervals;
    #endregion

    #region Scan State
    [Header("Scan Results")]
    [SerializeField] protected HealthManager detectedEnemy;
    [SerializeField] protected HealthManager detectedAlly;
    [SerializeField] protected Vector3 currentTarget;
    [SerializeField] protected int enemiesInView;
    #endregion

    #region Dependencies
    [Header("Component References")]
    [SerializeField] protected HealthManager linkedHealth;
    [SerializeField] protected Transform aimOffset;
    #endregion

    #region Runtime Variables
    protected float _scanTimer;
    #endregion

    #region Properties
    public HealthManager DetectedEnemy => detectedEnemy;
    public HealthManager DetectedAlly => detectedAlly;
    public Vector3 CurrentTarget => currentTarget;
    #endregion

    #region Direction and Distance
    public float DistanceEnemy
    {
        get
        {
            return (this.detectedEnemy != null) ? (transform.position - this.detectedEnemy.transform.position).magnitude : -1;
        }
    }
    public Vector3 DirectionEnemy
    {
        get
        {
            if (this.detectedEnemy != null)
            {
                return (this.detectedEnemy.transform.position - transform.position).normalized;
            }
            return Vector3.zero;
        }
    }
    public float DistanceAllied
    {
        get
        {
            return (this.detectedAlly != null) ? (transform.position - this.detectedAlly.transform.position).magnitude : -1;
        }
    }
    public Vector3 DirectionAllied
    {
        get
        {
            if (this.detectedAlly != null)
            {
                return (this.detectedAlly.transform.position - transform.position).normalized;
            }
            return Vector3.zero;
        }
    }

    public float DistanceTarget
    {
        get
        {
            return (transform.position - this.currentTarget).magnitude;
        }
    }
    public Vector3 DirectionTarget
    {
        get
        {
            return (currentTarget - transform.position).normalized;
        }
    }
    #endregion

    #region Initialization
    protected virtual void Awake()
    {
        LoadComponents();
    }

    protected virtual void LoadComponents()
    {
        if (linkedHealth == null)
        {
            linkedHealth = GetComponent<HealthManager>();
        }

        mainVision.Owner = linkedHealth;
        _scanTimer = 0;
        scanIntervals.InitializeBuffer();
    }
    #endregion

    #region Scan Logic
    public virtual void UpdateScan()
    {

        if (_scanTimer > scanIntervals.FloatBuffer[scanIntervals.CurrentIndex])
        {
            scanIntervals.MoveNext();
            Scan();
            _scanTimer = 0;
        }

        _scanTimer += Time.deltaTime;

        if (detectedEnemy != null && ((detectedEnemy.IsDead) || (detectedEnemy.IsVisible)))
        {
            detectedEnemy = null;
        }
    }

    public virtual void Scan()
    {
        if (linkedHealth.CurrentAttacker != null && linkedHealth.IsDead) return;
        detectedAlly = null;
        detectedEnemy = null;
        Collider[] colliders = Physics.OverlapSphere(transform.position, mainVision.MaxDistance, mainVision.ScanLayers);
        enemiesInView = 0;


        float min_dist = 10000000000f;

        for (int i = 0; i < colliders.Length; ++i)
        {

            GameObject obj = colliders[i].gameObject;

            if (this.IsNotIsThis(this.gameObject, obj))
            {
                HealthManager targetHealth = obj.GetComponent<HealthManager>();
                if (IsValidTarget(targetHealth))
                {
                    EvaluateTarget(targetHealth, ref min_dist);
                }

            }
        }
    }

    public virtual bool IsNotIsThis(GameObject obj1, GameObject obj2)
    {

        return (obj1.GetInstanceID() != obj2.GetInstanceID());
    }

    private bool IsValidTarget(HealthManager targetHealth)
    {
        return targetHealth != null &&
               targetHealth.gameObject.activeSelf &&
               !targetHealth.IsDead &&
               targetHealth.IsVisible &&
               mainVision.IsInSight(targetHealth.AimOffset);
    }

    private void EvaluateTarget(HealthManager target, ref float closestDistance)
    {
        if (IsAlly(target))
        {
            detectedAlly = target;
            return;
        }

        float dist = (transform.position - target.transform.position).magnitude;
        if (closestDistance > dist)
        {
            detectedEnemy = target;
            closestDistance = dist;

        }
        enemiesInView++;
    }

    protected virtual bool IsAlly(HealthManager target)
    {
        return linkedHealth.AlliedEntityGroups.Contains(target.EntityGroup);
    }
    #endregion
}